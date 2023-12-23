using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TimoriaSagaNetworkLibrary
{
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSizeLength = 2; // 패킷이 왔다는 것은 그것의 시작 2바이트는 크기고 그 다음 2바이트는 ID

        // sealed를 통해 OnReceive()를 PacketSession의 자식클래스가더이상 상속 받지 않게 해두자
        public sealed override int OnReceive(ArraySegment<byte> buffer)
        {
            int processedBytes = 0;
            int packetCount = 0;

            while (true)
            {
                // 최소 헤더 랭스값 파싱 가능한가?
                if (buffer.Count < HeaderSizeLength)
                    break;

                // 패킷이 완전체로 왔나?
                ushort thisPacketSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset); // 처음 2바이트(길이 뽑아옴)
                if (buffer.Count < thisPacketSize)
                    break;

                // 패킷을 안정적으로 받았으니 이 하나의 패킷을 처리하자
                OnReceivePacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, thisPacketSize));
                packetCount++;

                processedBytes += thisPacketSize;

                // 이거 필요함? 필요없어보이는데? OnRead에서 하잖아? 지워야 할듯 하다
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + thisPacketSize, buffer.Count - thisPacketSize);
            }

            if (packetCount > 1)
                Console.WriteLine($"다수의 패킷을 한묶음으로 받았음 : {packetCount} 개 패킷");

            return processedBytes;
        }

        public abstract void OnReceivePacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
    {
        protected Socket mcSocket;
        int mbDisconnected = 0; // 멀티 쓰레드 환경에서 쓰레드간 충돌관계를 막기 위한 동기화 플래그

        // 샌드를 위한 이벤트 인자는 클래스 멤버로 정의하자,
        // 이 클라이언트 세션의 모든 샌드를 위한 의미있는 인자인것
        protected SocketAsyncEventArgs mcSendArgs = new SocketAsyncEventArgs();
        protected Queue<ArraySegment<byte>> maSendQueue = new Queue<ArraySegment<byte>>();    // 샌드 큐(각각의 데이터그램)
        protected List<ArraySegment<byte>> maSendDataList = new List<ArraySegment<byte>>();   // 보내기 직전 각 데이터그램들을 나중에 한번에 보내려고 뭉치는 작업용

        protected SocketAsyncEventArgs mcReceiveArgs = new SocketAsyncEventArgs();

        // 리시브에 쓰일 버퍼를 클래스화 해둔것
        protected ReceiveBuffer mcReceiverBuffer = new ReceiveBuffer(65535);

        // 세션을 사용하기 위한 인터페이스
        public abstract void OnConnected(EndPoint endPoint);
        public abstract void OnDisconnected(EndPoint endPoint);
        public abstract int OnReceive(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);

        // 작업 동기화 플래그
        object mcLock = new object();

        //SocketEventArgPool mcArgumentPool = new SocketEventArgPool(1100);

        void Clear()
        {
            lock (mcLock)
            {
                maSendQueue.Clear();
                maSendDataList.Clear();
            }
        }

        public void Start()
        {
            mcReceiveArgs.Completed += OnReceiveCompleted;

            // 샌드를 위한 세팅 샌드 이벤트 인자의 핸들러 설정
            mcSendArgs.Completed += OnSendCompleted;
        }

        public void Enable(Socket socket)
        {
            mcSocket = socket;
            mbDisconnected = 0;

            RegisterReceive();
        }

        public void Send(ArraySegment<byte> sendBuffer)
        {
            // 동시에 Send가 불릴경우 엉키는 일을 막자
            lock (mcLock)
            {
                // 샌드 버퍼 세팅, 샌드버퍼가 읽히기 전에 다른 send 명령이 오면 버퍼를
                // 바꿔버릴 것이다. 이를 차곡 차곡 쌓아뒀다 샌드할때 한번에 플러쉬 하자
                maSendQueue.Enqueue(sendBuffer);

                if (maSendDataList.Count == 0)
                {
                    RegisterSend();
                }
            }
        }

        public void Send(List<ArraySegment<byte>> sendBufferList)
        {
            if (sendBufferList.Count == 0)
                return;

            // 동시에 Send가 불릴경우 엉키는 일을 막자
            lock (mcLock)
            {
                // 샌드 버퍼 세팅, 샌드버퍼가 읽히기 전에 다른 send 명령이 오면 버퍼를
                // 바꿔버릴 것이다. 이를 차곡 차곡 쌓아뒀다 샌드할때 한번에 플러쉬 하자
                foreach (ArraySegment<byte> sendBuff in sendBufferList)
                    maSendQueue.Enqueue(sendBuff);

                // 여기를 동시에 진입할 경우 문제가 있을 것도 같다 차후 처리
                if (maSendDataList.Count == 0)
                {
                    RegisterSend();
                }
            }
        }

        public void Disconnect()
        {
            // 쓰레드 동기화 기법
            // Session은 다른 Session과 레이스(경쟁) 상태에 있을 수 있기에 멀티 쓰레드 동기화에 신경써야 한다
            // 소켓 함수중 비동기 함수(AsyncAccept등)를 통해 콜백을 받는 이벤트 핸들러들은 이미 그 시점에 다른 쓰레드
            // 에서 돌고있다.

            // 참조하고 있는 mbDisconnected를 바꾸고 있는 동안은 다른 쓰레드에서 절대
            // 이 mbDisconnected에 읽기, 쓰기 접근을 할 수 없다.
            // 아토믹 하다, 변수 로딩, 값변경 등 각 작업이 쪼개지지 않는다
            // Exchange함수는 두번째 인자로 변수를 바꾸며 원래 값을 리턴한다
            if (Interlocked.Exchange(ref mbDisconnected, 1) == 1)
                return;

            if (mcSocket != null)
            {
                // 컨텐츠 레이어쪽에(자식 클래스) 이벤트 효과를 만들어준다
                OnDisconnected(mcSocket.RemoteEndPoint);

                //mcReceiveArgs.Completed -= OnReceiveCompleted;
                //mcSendArgs.Completed -= OnSendCompleted;

                mcReceiveArgs.UserToken = null;
                mcSendArgs.UserToken = null;

                mcReceiveArgs.RemoteEndPoint = null;
                mcSendArgs.RemoteEndPoint = null;

                //mcReceiveArgs.BufferList = null;
                //mcSendArgs.BufferList = null;

                //mcReceiveArgs.SetBuffer(null, 0,0);
                //mcSendArgs.SetBuffer(null,0,0);

                mcSocket.Shutdown(SocketShutdown.Both); // 이줄 안넣어도 됨
                mcSocket.Close();

                // 샌드큐와, 샌드 데이터 리스트 잔류하는 것들 정리
                Clear();

                mcSocket = null;
            }
        }

        #region 네트워크 통신 관련

        void RegisterSend()
        {
            if (mbDisconnected == 1) return;

            while (maSendQueue.Count > 0)
            {
                ArraySegment<byte> buffer = maSendQueue.Dequeue();

                // 참고로 여기서 인자형인 ArraySegment는 클래스가 아니라 스트럭이다
                // 그래서 힙에 저장되지 않고 스택에 저장되있을 것이다. 클래스와 스트럭은 이게 다른듯
                // 그럼 힙에 있지 않으니 레퍼런스 참조(포인터 참조)가 되나?
                // ArraySegment자체가 레퍼런스일듯

                // 이렇게 ArraySegment<>()를 사용하는 이유는 C#이 포인터 처리가 안되기때문이라고
                // C++의 경우 배열의 일부를 전달하고자 할때 그 배열의 시작주소에서 몇번 더한 주소값을 주면 된다
                // C#은 그런게 없으니까 배열의 어느부분을 전달해야 하는지 이 시작값과 Length를 명확히 정의한
                // ArraySegment 형 자료가 필요한 것.
                maSendDataList.Add(buffer);
            }

            // 여기서 sendDataList라는 변수를 우리가 직접 만들어 넣어준다.
            // 이벤트 인자(mcSendArgs)에 있는 버퍼리스트 BufferList에는 new 처리가 되어있지 않다
            // 그냥 참조변수(포인터기능)인것, 이는 우리가 리스트를 이렇게 꼼꼼히 만들어줘야 하는 수고스러움이
            // 있지만 대신, 자유롭게 사용할수 있어 보인다 제한 사항이 없는 것.
            mcSendArgs.BufferList = maSendDataList;

            try
            {
                bool isDeferred = mcSocket.SendAsync(mcSendArgs);

                if (isDeferred == false)
                {
                    OnSendCompleted(null, mcSendArgs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RegisterSend Failed {ex}");
            }

        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            // OnSendCompled 같은 경우 흐름상 겹칠일이 없다 이미 앞 흐름(Send 함수)에서 락을 걸고 처리하기 때문
            // 다만 공용르로 쓰이는 이벤트 핸들러 함수이기에 다른 코드 흐름 어딘가 이 함수가 별도로 등록되 사용될 수도있다.
            // 엉킬수도 있는 것. 그래서 락을 안풀었다.
            lock (mcLock)
            {
                // 참고로 BytesTransferred는 송수신 모든 경우에 사용된다
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        // 이순간 리스트에 있는건 인자 내부에서 보낼꺼 다보내고 왔을 것이다
                        mcSendArgs.BufferList = null;

                        // 클리어 함으로써 maSendDataList의 Count가 0이 되면 이는 위쪽에서 플래그로 쓴다
                        maSendDataList.Clear();

                        OnSend(mcSendArgs.BytesTransferred); // 컨텐츠단 콜백

                        // 보낼게 또 쌓였으면 계속 보내자
                        if (maSendQueue.Count > 0)
                        {
                            RegisterSend();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {ex}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }

        void RegisterReceive()
        {
            if (mbDisconnected == 1) return;

            mcReceiverBuffer.Clean();
            ArraySegment<byte> segment = mcReceiverBuffer.FreeSegment;
            mcReceiveArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {
                bool isDeferred = mcSocket.ReceiveAsync(mcReceiveArgs);

                if (isDeferred == false)
                {
                    OnReceiveCompleted(null, mcReceiveArgs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RegisterReceive Failed {ex}");
            }
        }

        void OnReceiveCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    // 보낸 내용에 따라서 버퍼의 Write 포인터 이동
                    if (mcReceiverBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }

                    // 컨텐츠쪽으로 받은 데이터를 플러싱
                    int processedByteNum = OnReceive(mcReceiverBuffer.DataSegment);
                    if (processedByteNum < 0 || processedByteNum > mcReceiverBuffer.DataSize)
                    {
                        Disconnect();
                        return;
                    }

                    // 처리한 내용에 따라서 버퍼의 Read 포인터 변경
                    if (mcReceiverBuffer.OnRead(processedByteNum) == false)
                    {
                        Disconnect();
                        return;
                    }

                    RegisterReceive();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"OnReceiveCompleted Failed {ex}");
                }
            }
            else
            {
                Disconnect();
            }
        }
        #endregion
    }
}

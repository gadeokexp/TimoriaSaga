using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TimoriaSagaNetworkLibrary
{
    public class Connector
    {
        // 컨텐츠로부터 특정 타입(Session의 자식)을 받아 만들기 위한 팩토리 함수
        Func<Session> mfSessionFactory;

        // 커넥트를 위한 소캣은 멤버 변수로 관리하지 않는다
        // 이걸 쓰는 주체가 여러 커넥션을 유지해야 될지 모르는 경우
        // 서버가 커넥터를 사용할경우
        // 커넥터 객체가 멤버로 소켓을 가지면 개당 하나의 커넥션만 유지하기 때문

        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory, int connectCount = 1) 
        {
            for (int i = 0; i < connectCount; i++)
            {
                Socket clientSideSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // 이벤트 인자 세팅
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnConnectCompleted;
                args.RemoteEndPoint = endPoint;

                // 이벤트아규먼트에 사용자에게 할당된 영역을 이용해서 클라이언트 소켓 전달
                // 이는 그냥 클래스 멤버로 만들어서 사용해도 된다.
                // 이래도 되고 저래도 되는데 심심해서(다양한 방법을 써보고 싶어서) 해본 것
                args.UserToken = clientSideSocket;

                mfSessionFactory = sessionFactory;

                RegisterConnect(args);
            }
        }

        public void RegisterConnect(SocketAsyncEventArgs args)
        {
            Socket clientSideSocket = args.UserToken as Socket;

            if (clientSideSocket == null)
            {
                return;
            }

            bool isDeferred = clientSideSocket.ConnectAsync(args);
            if (isDeferred == false)
            {
                OnConnectCompleted(null, args);
            }
        }

        public void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                Session session = mfSessionFactory.Invoke();
                session.Start();
                session.Enable(args.ConnectSocket);      // 참고로 UserToken에도 같은게 있다
                session.OnConnected(args.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine($"OnConnectCompleted Fail : {args.SocketError}");
            }
        }
    }
}

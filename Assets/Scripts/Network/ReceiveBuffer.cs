using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimoriaSagaNetworkLibrary
{
    public class ReceiveBuffer
    {
        // 포인터기능을 조금 첨가해서 잘라쓰거나 인덱싱할수있는 버퍼
        ArraySegment<byte> mcPointableBuffer;

        int miReadPointer;
        int miWritePointer;

        public ReceiveBuffer(int bufferSize)
        {
            mcPointableBuffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public int DataSize { get { return miWritePointer - miReadPointer; } }
        public int FreeSize { get { return mcPointableBuffer.Count - miWritePointer; } }
        // 참고로 +1 안해줘도 됨 miWritePointer가 0부터 시작

        public ArraySegment<byte> DataSegment
        {
            get { return new ArraySegment<byte>(mcPointableBuffer.Array, mcPointableBuffer.Offset + miReadPointer, DataSize); }
        }

        public ArraySegment<byte> FreeSegment
        {
            get { return new ArraySegment<byte>(mcPointableBuffer.Array, mcPointableBuffer.Offset + miWritePointer, FreeSize); }
        }

        public void Clean()
        {
            int dataSize = DataSize;
            if (dataSize == 0)
            {
                miReadPointer = 0;
                miWritePointer = 0;
            }
            else
            {
                // 안 읽고 남아있는데이터를 앞으로 땡겨줌
                Array.Copy(
                    mcPointableBuffer.Array,
                    mcPointableBuffer.Offset + miReadPointer,
                    mcPointableBuffer.Array,
                    mcPointableBuffer.Offset,
                    dataSize);

                miReadPointer = 0;
                miWritePointer = dataSize;
            }
        }

        public bool OnRead(int numOfBytes)
        {
            // 패킷을 성공적으로 읽었을 경우 불리는 함수
            if (numOfBytes > DataSize) return false;

            miReadPointer += numOfBytes;
            return true;
        }

        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize) return false;

            miWritePointer += numOfBytes;
            return true;
        }
    }
}

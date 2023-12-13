using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimoriaSagaNetworkLibrary
{
    public class SendBufferPool
    {
        //public static int ChunkSize { get; set; } = 4096 * 32;
        public static int ChunkSize { get; set; } = 65535 * 100;

        public static SendBuffer CurrentBuffer = new SendBuffer(ChunkSize);

        static object mcLock = new object();

        public static ArraySegment<byte> UsingBufferStart(int reserveSize)
        {
            lock (mcLock)
            {
                return CurrentBuffer.UsingBufferStart(reserveSize);
            }
        }
    }

    public class SendBuffer
    {
        byte[] maBuffer;
        public int miWritePointer = 0;

        public int FreeSize => maBuffer.Length - miWritePointer;

        public SendBuffer(int chunkSize)
        {
            maBuffer = new byte[chunkSize];
        }

        public ArraySegment<byte> UsingBufferStart(int reserveSize)
        {
            if (reserveSize > FreeSize)
                miWritePointer = 0;

            int writePointer = miWritePointer;
            miWritePointer += reserveSize;

            return new ArraySegment<byte>(maBuffer, writePointer, reserveSize);
        }
    }
}

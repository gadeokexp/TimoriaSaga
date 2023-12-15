using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TimoriaSagaNetworkLibrary;

namespace TimoriaSagaDummyClient
{
    class ClientSideSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            UnityEngine.Debug.Log($"서버에 접속 됨");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            UnityEngine.Debug.Log($"서버와 접속 끊김");
        }

        public override void OnReceivePacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnReceivePacket(this, buffer, (session, packet) => PacketQueue.Instance.Push(packet));
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}

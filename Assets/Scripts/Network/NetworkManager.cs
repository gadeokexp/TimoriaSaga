using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TimoriaSagaDummyClient;
using TimoriaSagaNetworkLibrary;
using UnityEngine;

public class NetworkManager : MonoSingleton<NetworkManager>
{
    ClientSideSession mcSession = new ClientSideSession();
    public string UniqueId;

    public void Init()
    {
        // 로컬 컴퓨터의 호스트 이름(DNS)을 가져온다.
        // 로컬 DNS 이름 Localhost와 그 IP를 찾는 과정
        // 이 로컬DNS 이름이 다른것으로 바껴있으면 그것을 가져올 것이다
        // 이 코드는 차후(출시전) 서버의 도메인 네임으로 바뀌어야 할 것이다.
        string hostDNS = Dns.GetHostName();
        Dns.GetHostEntry(hostDNS);
        IPHostEntry serverIPList = Dns.GetHostEntry(hostDNS);

        // 호스트가 거대 서버일 경우 여러 아이피로 사용자 트래픽을 분산해서 받는다 그래서 리스트로 되있다
        // 우리는 첫번째 목록만 참고하면 된다.
        IPAddress serverIP = serverIPList.AddressList[0];

        // 포트 번호를 포함에서 소캣 인자로 쓰기 위한 최종 종단 주소 
        IPEndPoint serverEndPoint = new IPEndPoint(serverIP, 9993); // 포트번호 9993을 쓰자

        Connector connector = new Connector();

        connector.Connect(serverEndPoint,
            () => { return mcSession; },
            1);
    }

    // Update is called once per frame
    public void Update()
    {
        for (IPacket packet = PacketQueue.Instance.Pop(); packet != null; packet = PacketQueue.Instance.Pop())
        {
            PacketManager.Instance.HandlePacket(mcSession, packet);
        }
    }

    public void OnDisable()
    {
        mcSession.Disconnect();
    }

    public void Send(ArraySegment<byte> sendBuffer)
    {
        mcSession.Send(sendBuffer);
    }
}

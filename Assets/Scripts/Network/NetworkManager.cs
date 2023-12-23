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
        // ���� ��ǻ���� ȣ��Ʈ �̸�(DNS)�� �����´�.
        // ���� DNS �̸� Localhost�� �� IP�� ã�� ����
        // �� ����DNS �̸��� �ٸ������� �ٲ������� �װ��� ������ ���̴�
        // �� �ڵ�� ����(�����) ������ ������ �������� �ٲ��� �� ���̴�.
        string hostDNS = Dns.GetHostName();
        Dns.GetHostEntry(hostDNS);
        IPHostEntry serverIPList = Dns.GetHostEntry(hostDNS);

        // ȣ��Ʈ�� �Ŵ� ������ ��� ���� �����Ƿ� ����� Ʈ������ �л��ؼ� �޴´� �׷��� ����Ʈ�� ���ִ�
        // �츮�� ù��° ��ϸ� �����ϸ� �ȴ�.
        IPAddress serverIP = serverIPList.AddressList[0];

        // ��Ʈ ��ȣ�� ���Կ��� ��Ĺ ���ڷ� ���� ���� ���� ���� �ּ� 
        IPEndPoint serverEndPoint = new IPEndPoint(serverIP, 9993); // ��Ʈ��ȣ 9993�� ����

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

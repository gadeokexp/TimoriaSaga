using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketQueue : Singleton<PacketQueue>
{
    // ����Ƽ���� ���� ������� �޴� ��Ŷ �����͵��� update�� ���� ����
    // �������� mono Ŭ���� �����鿡 �Ժη� ������ �� ����
    // ��Ŷ ť�� ���ú� �����尡 ���� ��Ŷ���� �׾Ƶΰ�
    // ����Ƽ ��� update �Լ��� ���� ������ �ϴ� ������ ���������� �Ѵ�.

    // ���ŵ� ��Ŷ�� ����Ƽ ���ξ����忡�� ����Ҽ��ְ� ����ִ� ť
    Queue<IPacket> maPacketQueue = new Queue<IPacket>();

    // ť�� ���ξ����忡�� �����⵵ �ϰ�, ��Ʈ��ũ���� ���꾲���忡�� �ֱ⵵ �Ұ��̴�
    // ����ȭ ������ �ʿ��� ��
    object mcLock = new object();

    public void Push(IPacket packet)
    {
        lock (mcLock)
        {
            maPacketQueue.Enqueue(packet);
        }
    }

    public IPacket Pop()
    {
        lock (mcLock)
        {
            if (maPacketQueue.Count == 0) return null;

            return maPacketQueue.Dequeue();
        }
    }
}

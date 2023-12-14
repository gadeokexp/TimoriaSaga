using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketQueue : Singleton<PacketQueue>
{
    // 유니티에서 서브 쓰레드로 받는 패킷 데이터들은 update가 도는 메인
    // 쓰레드의 mono 클래스 변수들에 함부로 접근할 수 없다
    // 패킷 큐는 리시브 쓰레드가 받은 패킷들을 쌓아두고
    // 유니티 모노 update 함수가 꺼내 쓰도록 하는 일종의 관문역할을 한다.

    // 수신된 패킷을 유니티 메인쓰레드에서 사용할수있게 담아주는 큐
    Queue<IPacket> maPacketQueue = new Queue<IPacket>();

    // 큐는 메인쓰레드에서 꺼내기도 하고, 네트워크같은 서브쓰레드에서 넣기도 할것이다
    // 동기화 세팅이 필요한 것
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

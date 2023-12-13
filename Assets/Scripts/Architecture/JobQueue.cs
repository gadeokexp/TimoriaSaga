using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JobQueue
{
    // 서버를 식당에 비유하자면 주방에는 모든 일꾼이 있을 필요가 없다
    // 주방은 좁으니까 (한정된 자원)
    // 주방장이 전달되는 주문내용을 바탕으로 혼자 만드는게 더 효과적인 것.
    // 이렇게 다수가 소수에게 일감만 토스하는걸 커맨드 패턴이라고 한다 
    Queue<Action> maJobQueue = new Queue<Action>();

    object mcLock = new object();

    public void PushJob(Action job)
    {
        lock (mcLock)
        {
            maJobQueue.Enqueue(job);
        }
    }

    public Action Pop()
    {
        lock (mcLock)
        {
            if (maJobQueue.Count == 0)
            {
                return null;
            }

            return maJobQueue.Dequeue();
        }
    }

    public int GetCount()
    {
        return maJobQueue.Count;
    }

    public void Clear()
    {
        maJobQueue.Clear();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class JobQueue
{
    // ������ �Ĵ翡 �������ڸ� �ֹ濡�� ��� �ϲ��� ���� �ʿ䰡 ����
    // �ֹ��� �����ϱ� (������ �ڿ�)
    // �ֹ����� ���޵Ǵ� �ֹ������� �������� ȥ�� ����°� �� ȿ������ ��.
    // �̷��� �ټ��� �Ҽ����� �ϰ��� �佺�ϴ°� Ŀ�ǵ� �����̶�� �Ѵ� 
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
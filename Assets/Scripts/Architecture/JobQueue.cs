using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobUnit<T>
{
    Action<T> _actinonUnit;
    T Arg;

    public JobUnit(Action<T> actinon, T arg)
    {
        _actinonUnit = actinon;
        Arg = arg;
    }

    public void Execute()
    {
        _actinonUnit.Invoke(Arg);
    }
}

public class JobQueue<T>
{
    public Queue<JobUnit<T>> _jobQueue = new Queue<JobUnit<T>>();

    public void SchduleJob(Action<T> action, T t1)
    {
        PushJob(new JobUnit<T>(action, t1));
    }

    public void PushJob(JobUnit<T> job)
    {
        _jobQueue.Enqueue(job);
    }

    public int GetCount()
    {
        return _jobQueue.Count;
    }

    public JobUnit<T> Pop()
    {
        if (_jobQueue.Count == 0)
        {
            return null;
        }

        return _jobQueue.Dequeue();
    }

    public void Excute()
    {
        if (_jobQueue.Count == 0)
        {
            return;
        }

        _jobQueue.Dequeue().Execute();
    }
}
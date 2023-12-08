using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteState<T> where T : class
{
    protected T owner;
    public T Owner { get { return owner; } set { owner = value; } }

    protected int _id;
    public int ID => _id;

    protected HashSet<int> transferableState = new();

    public Action Enter;
    public Action Exit;
    public Action Update;

    public virtual bool isTransforable(int stateID)
    {
        return transferableState.Contains(stateID);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteState<T> where T : class
{
    protected T owner;
    public T Owner { get { return owner; } set { owner = value; } }

    public Action Enter;
    public Action Exit;
    public Action Update;
}

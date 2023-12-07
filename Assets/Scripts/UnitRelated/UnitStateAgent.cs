using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using UnityEngine.XR;
using static UnityEngine.UI.GridLayoutGroup;

enum UnitState
{
    Idle,
    Move,
    Hit,
    Beaten,
    Die,
    UnitStateLength
}

public class UnitStateAgent<T> : MonoBehaviour where T : class
{
    protected FiniteState<T> currentState;
    protected FiniteState<T>[] states;
    
    public void StateInit(T owner)
    {
        currentState = null;

        states = new FiniteState<T>[(int)UnitState.UnitStateLength];
        states[(int)UnitState.Idle] = new IdleState<T>(owner);
        states[(int)UnitState.Move] = new MoveState<T>(owner);
        states[(int)UnitState.Hit] = new HitState<T>(owner);
        states[(int)UnitState.Beaten] = new BeatenState<T>(owner);
        states[(int)UnitState.Die] = new DieState<T>(owner);

        ChangeState(states[(int)UnitState.Idle]);
    }

    public void UpdateState()
    {
        if(currentState != null && currentState.Update != null)
        {
            currentState.Update();
        }
    }

    public void ChangeState(FiniteState<T> newState)
    {
        if (newState == null) return;

        if (currentState != null && currentState.Exit != null)
        {
            currentState.Exit();
        }

        currentState = newState;

        if (currentState.Enter != null)
        {
            currentState.Enter();
        }
    }
}

public class IdleState<T> : FiniteState<T> where T : class
{
    public IdleState(T newOwner)
    {
        owner = newOwner;
    }
}

public class MoveState<T> : FiniteState<T> where T : class
{
    public MoveState(T newOwner)
    {
        owner = newOwner;
    }

    public InputManager.MoveEvent Execute;
}

public class HitState<T> : FiniteState<T> where T : class
{
    public HitState(T newOwner)
    {
        owner = newOwner;
    }
}

public class BeatenState<T> : FiniteState<T> where T : class
{
    public BeatenState(T newOwner)
    {
        owner = newOwner;
    }
}

public class DieState <T> : FiniteState<T> where T : class
{
    public DieState(T newOwner)
    {
        owner = newOwner;
    }
}

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
    public FiniteState<T>[] States => states;
    
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

    public void AgentUpdate()
    {
        if(currentState != null && currentState.Update != null)
        {
            currentState.Update();
        }
    }

    public void ChangeState(FiniteState<T> newState)
    {
        if (newState == null) return;

        if (currentState != null)
        {
            // 현상태와 바뀌는 상태가 같을경우 여기서 처리해주자
            if(!currentState.isTransforable(newState.ID))
            {
                return;
            }

            if (currentState.Exit != null)
            {
                currentState.Exit();
            }
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
        _id = (int)UnitState.Idle;

        owner = newOwner;

        transferableState.Add((int)UnitState.Move);
        transferableState.Add((int)UnitState.Hit);
        transferableState.Add((int)UnitState.Beaten);
        transferableState.Add((int)UnitState.Die);
    }
}

public class MoveState<T> : FiniteState<T> where T : class
{
    public MoveState(T newOwner)
    {
        _id = (int)UnitState.Move;

        owner = newOwner;

        transferableState.Add((int)UnitState.Idle);
        transferableState.Add((int)UnitState.Hit);
        transferableState.Add((int)UnitState.Beaten);
        transferableState.Add((int)UnitState.Die);
    }
}

public class HitState<T> : FiniteState<T> where T : class
{
    public HitState(T newOwner)
    {
        _id = (int)UnitState.Hit;

        owner = newOwner;

        //transferableState.Add((int)UnitState.Idle);
        //transferableState.Add((int)UnitState.Move);
        transferableState.Add((int)UnitState.Beaten);
        transferableState.Add((int)UnitState.Die);

        Enter += EnterBase;
    }

    private void EnterBase()
    {
        // 이렇게 기본 Enter함수 외에 추가 함수를 붙일 수 있다
    }
}

public class BeatenState<T> : FiniteState<T> where T : class
{
    public BeatenState(T newOwner)
    {
        _id = (int)UnitState.Beaten;

        owner = newOwner;
    }
}

public class DieState <T> : FiniteState<T> where T : class
{
    public DieState(T newOwner)
    {
        _id = (int)UnitState.Die;

        owner = newOwner;
    }
}

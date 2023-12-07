using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;

public class UnitSoul : UnitStateAgent<UnitSoul>
{
    [SerializeField]
    public bool isMyCharacter = true;

    private Animator animator;

    Vector3 _lookingDirection;
    Coroutine _rotationTolook;

    void Start()
    {
        StateInit(this);

        MoveState<UnitSoul> moveState = states[(int)UnitState.Move] as MoveState<UnitSoul>;
        moveState.Execute = OnMove;
        moveState.Enter = OnMoveEnter;
        moveState.Exit = OnMoveExit;

        IdleState<UnitSoul> idleState = states[(int)UnitState.Idle] as IdleState<UnitSoul>;
        idleState.Enter = OnIdleEnter;

        if (isMyCharacter)
        {
            InputManager.Instance.OnMoveInput += moveState.Execute;
        }

        animator = GetComponent<Animator>();
    }

    void OnMove(float XInput, float ZInput)
    {
        if (XInput == 0f && ZInput == 0f)
        {
            if (currentState == states[(int)UnitState.Move])
            {
                ChangeState(states[(int)UnitState.Idle]);
            }
        }
        else
        {
            if (currentState != states[(int)UnitState.Move])
            {
                ChangeState(states[(int)UnitState.Move]);
            }
            // 사용자 인풋이나, 패킷에 수신등의 이벤트등으로 콜된다.
            _lookingDirection = new Vector3(XInput, 0, ZInput).normalized;

            transform.position += _lookingDirection * Time.deltaTime * 3;

            if (_rotationTolook == null)
            {
                _rotationTolook = StartCoroutine(RotationtoLook());
            }
        }
    }

    void OnMoveEnter()
    {
        animator.Play("Run_SwordShield");
    }

    void OnMoveExit()
    {
        animator.StopPlayback();
    }

    void OnIdleEnter()
    {
        animator.Play("Idle_SwordShield");
    }

    IEnumerator RotationtoLook()
    {
        float accumulatedDelta = 0;
        Vector3 lookingDirection = _lookingDirection;
        Quaternion lookingDirectionAngle = Quaternion.LookRotation(lookingDirection);

        while (true)
        {
            accumulatedDelta += Time.deltaTime * 2;

            // 회전 방향이 변했을 경우 리셋
            if(_lookingDirection != lookingDirection)
            {
                lookingDirection = _lookingDirection;
                lookingDirectionAngle = Quaternion.LookRotation(lookingDirection);
                accumulatedDelta = 0;
            }

            // 회전이 끝났을 경우 정리
            if(accumulatedDelta >= 0.98f)
            {
                transform.rotation = lookingDirectionAngle;
                accumulatedDelta = 0;
                _rotationTolook = null;
                break;
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, lookingDirectionAngle, accumulatedDelta);
            yield return null;
        }
    }
}

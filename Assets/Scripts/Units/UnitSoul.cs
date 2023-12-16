using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.Windows;

public class UnitSoul : UnitStateAgent<UnitSoul>
{
    [SerializeField]
    public bool isMyCharacter = true;

    private Animator animator;

    Vector3 _lookingDirection;
    Coroutine _rotationTolook;

    UsedInput _input;

    // 대각 이동에 관한 키보드입력 보정처리
    float _diagonalMovementDelta = 0;
    float _prevDiagonalX = 0;
    float _prevDiagonalZ = 0;

    void Start()
    {
        StateInit(this);

        MoveState<UnitSoul> moveState = states[(int)UnitState.Move] as MoveState<UnitSoul>;
        moveState.Enter += OnMoveEnter;
        moveState.Exit += OnMoveExit;
        moveState.Update += OnMoveUpdate;

        IdleState<UnitSoul> idleState = states[(int)UnitState.Idle] as IdleState<UnitSoul>;
        idleState.Enter += OnIdleEnter;

        HitState<UnitSoul> hitState = states[(int)UnitState.Hit] as HitState<UnitSoul>;
        hitState.Enter += OnHitEnter;
        hitState.Exit += OnHitExit;

        if (isMyCharacter)
        {
            InputManager.Instance.OnInput += OnInput;
            _input = InputManager.Instance.GameInput;
        }

        animator = GetComponent<Animator>();
    }

    void OnInput(ref UsedInput input)
    {
        if (input.Function1 && currentState != states[(int)UnitState.Hit])
        {
            // 입력 대응 1순위
            // 공격키가 눌렸고 현재 공격상태가 아니면 공격상태로 전환한다.
            ChangeState(states[(int)UnitState.Hit]);
        }
        else if((input.XInput != 0f || input.ZInput != 0f))
        {
            // 입력 대응 2순위
            // 이동키가 눌렸고 현재 이동상태가  아니면 이동상태로 전환한다.
            ChangeState(states[(int)UnitState.Move]);
        }
        else
        {
            // 아무 키도 안눌렸으면 아이들 상태로 전환한다.
            ChangeState(states[(int)UnitState.Idle]);
        }

        if (currentState != null && currentState.Update != null)
        {
            currentState.Update();
        }
    }

    void OnMoveEnter()
    {
        animator.SetInteger("currentState", (int)UnitState.Move);
        _lookingDirection = new Vector3(_input.XInput, 0, _input.ZInput).normalized;
        _rotationTolook = StartCoroutine(RotationtoLook());

        
    }

    void OnMoveExit()
    {
        currentState = null;
    }

    float _movePacketInterval = 0.05f;

    void OnMoveUpdate()
    {
        float deltaTime = Time.deltaTime;

        if(isMyCharacter)
        {
            if (_input.XInput != 0f && _input.ZInput != 0f)
            {
                _prevDiagonalX = _input.XInput;
                _prevDiagonalZ = _input.ZInput;
                _diagonalMovementDelta = 0f;
            }
            else if (_diagonalMovementDelta < 0.1f)
            {
                _diagonalMovementDelta += deltaTime;
            }

            if (_input.DirectionChanged)
            {
                _lookingDirection = new Vector3(_input.XInput, 0, _input.ZInput).normalized;

                if (_rotationTolook == null)
                {
                    _rotationTolook = StartCoroutine(RotationtoLook());
                }
            }
        }
        else
        {

        }

        transform.position += _lookingDirection * deltaTime * 3;
        _movePacketInterval -= deltaTime;

        if (_movePacketInterval < 0)
        {
            SendMovePacket();
            _movePacketInterval = 0.05f;
        }
    }

    void OnIdleEnter()
    {
        if (_diagonalMovementDelta < 0.1f)
        {
            _lookingDirection = new Vector3(_prevDiagonalX, 0, _prevDiagonalZ).normalized;
            _diagonalMovementDelta = 0.1f;
        }

        animator.SetInteger("currentState", (int)UnitState.Idle);
    }

    void OnHitEnter()
    {
        animator.SetInteger("currentState", (int)UnitState.Hit);
        StartCoroutine(WaitForSwing());
    }

    void OnHitExit()
    {
        currentState = null;
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
            if (_lookingDirection != lookingDirection)
            {
                lookingDirection = _lookingDirection;
                lookingDirectionAngle = Quaternion.LookRotation(lookingDirection);
                accumulatedDelta = 0;
            }

            // 회전이 끝났을 경우 정리
            if (accumulatedDelta >= 0.98f)
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

    IEnumerator WaitForSwing()
    {
        yield return new WaitForSeconds(0.6f);
        currentState = null;
    }

    protected void SendMovePacket()
    {
            CTS_Move movePacket = new CTS_Move();
            movePacket.positionX = transform.position.x;
            movePacket.positionY = transform.position.y;
            movePacket.positionZ = transform.position.z;
            movePacket.directionX = _lookingDirection.x;
            movePacket.directionZ = _lookingDirection.z;
            movePacket.timeStamp = 33333333;

            NetworkManager.Instance.Send(movePacket.Write());
    }

}

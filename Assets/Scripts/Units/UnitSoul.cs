using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class UnitSoul : UnitStateAgent<UnitSoul>
{
    [SerializeField]
    public bool isMyCharacter = true;

    private Animator animator;

    // 이동과 회전
    public Vector3 LookingDirection { 
        get => _lookingDirection; 
        set 
        { 
            if(_lookingDirection.x != value.x || _lookingDirection.z != value.z)
            {
                _isDirectionChanged = true;
                _lookingDirection = value;
            }
        }
    }
    Vector3 _lookingDirection;
    bool _isDirectionChanged = false;
    Coroutine _rotationTolook;

    public Vector3 TargetPosition;

    UsedInput _input;

    // 대각 이동
    float _diagonalMovementDelta = 0.1f;
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

    void Update()
    {
        if (currentState != null && currentState.Update != null)
        {
            currentState.Update();
        }
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
    }

    void OnMoveEnter()
    {
        animator.SetInteger("currentState", (int)UnitState.Move);

        if (_input != null)
        {
            _lookingDirection = new Vector3(_input.XInput, 0, _input.ZInput).normalized;
            _rotationTolook = StartCoroutine(RotationtoLook());
        }
    }

    void OnMoveExit()
    {
        if (!isMyCharacter)
        {
            StartCoroutine(ClearPosition());
        }

        currentState = null;
    }

    float _moveInterval = 1f;
    Vector3 _moveRoute = Vector3.zero;
    Vector3 _moveStart = Vector3.zero;

    void OnMoveUpdate()
    {
        float deltaTime = Time.deltaTime;

        if (isMyCharacter)
        {
            _moveInterval += deltaTime * 20; // 0.05초에 한번씩 패킷을 주거나 이동처리를 한다

            // 키보드로 대각 방향이동 후 감도가 너무 높아서 대각으로 안선다.
            // 감도를 줄이기 위한 코드
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

            // 이동방향이 바뀐경우 회선 처리
            if (_input.DirectionChanged)
            {
                _lookingDirection = new Vector3(_input.XInput, 0, _input.ZInput).normalized;

                if (_rotationTolook == null)
                {
                    _rotationTolook = StartCoroutine(RotationtoLook());
                }
            }

            // 키보드 이동
            transform.position += _lookingDirection * deltaTime * 3;

            // 이동 보고
            if (_moveInterval > 1f)
            {
                SendMovePacket();
                _moveInterval = 0f;
            }
        }
        else
        {
            // 내캐릭이 아닌경우 서버의 오더에 따라 이동
            _moveInterval += deltaTime *20;

            if ( _moveRoute != TargetPosition)
            {
                _moveRoute = TargetPosition;
                _moveStart = transform.position;
                _moveInterval = deltaTime * 20;
            }

            if (_isDirectionChanged && _rotationTolook == null)
            {
                _rotationTolook = StartCoroutine(RotationtoLook());
            }

            if (_moveInterval < 1f)
            {
                transform.position = Vector3.Lerp(_moveStart, _moveRoute, _moveInterval);
            }
            else
            {
                transform.position = _moveRoute;
            }
        }
    }

    void OnIdleEnter()
    {
        if (_diagonalMovementDelta < 0.1f)
        {
            _lookingDirection = new Vector3(_prevDiagonalX, 0, _prevDiagonalZ).normalized;
            _diagonalMovementDelta = 0.1f;
        }

        SendIdlePacket();
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
        transform.rotation = lookingDirectionAngle; 

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

        if(!isMyCharacter) _isDirectionChanged = false;
    }

    IEnumerator ClearPosition()
    {
        while (true)
        {
            float deltaTime = Time.deltaTime;
            // 내캐릭이 아닌경우 서버의 오더에 따라 이동
            _moveInterval += deltaTime * 20;

            if (_moveRoute != TargetPosition)
            {
                _moveRoute = TargetPosition;
                _moveStart = transform.position;
                _moveInterval = deltaTime * 20;
            }

            if (_moveInterval < 1f)
            {
                transform.position = Vector3.Lerp(_moveStart, _moveRoute, _moveInterval);
            }
            else
            {
                yield break;
            }
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

    protected void SendIdlePacket()
    {
        CTS_Idle idlePacket = new CTS_Idle();
        idlePacket.directionX = _lookingDirection.x;
        idlePacket.directionZ = _lookingDirection.z;
        idlePacket.timeStamp = 33333333;

        NetworkManager.Instance.Send(idlePacket.Write());
    }

}

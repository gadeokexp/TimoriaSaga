using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    Dictionary<int, Collider> _myCharactersCollision;

    public int ID = 0;

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

        BeatenState<UnitSoul> beatenState = states[(int)UnitState.Beaten] as BeatenState<UnitSoul>;
        beatenState.Enter += OnBeatenEnter;
        beatenState.Exit += OnBeatenExit;

        if (isMyCharacter)
        {
            InputManager.Instance.OnInput += OnInput;
            _input = InputManager.Instance.GameInput;

            _myCharactersCollision = new Dictionary<int, Collider>();
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
        if (input.Function1)
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
            // 이미 아이들 상태이면 전환되지 않는다.
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
            //float conTheta = _CollisionDirection.x * _lookingDirection.x + _CollisionDirection.z * _lookingDirection.z;

            if(CheckCollions())
            {
                // 키보드 이동
                //transform.position += (_lookingDirection - _CollisionDirection * conTheta) * deltaTime * 3;
                transform.position += (_lookingDirection - _fixedDirection) * deltaTime * 3;
            }

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

        if(isMyCharacter)
        {
            _myCharactersCollision.Clear();
            SendIdlePacket();
        }

        animator.SetInteger("currentState", (int)UnitState.Idle);
    }

    void OnHitEnter()
    {
        if(isMyCharacter)
        {
            SendSkillPacket();
            StartCoroutine(WaitForSwing(0.6f));
        }
        else
        {
            StartCoroutine(WaitForSwing(0.3f));
            currentState = null;
        }

        int currentMotion = animator.GetInteger("currentState");

        if (currentMotion == 2)
        {
            animator.SetInteger("currentState", 201);
        }
        else
        {
            animator.SetInteger("currentState", (int)UnitState.Hit);
        }
    }

    void OnHitExit()
    {
        currentState = null;
    }

    void OnBeatenEnter()
    {
        if(currentState.ID == (int)UnitState.Beaten)
        {
            animator.Play("GetHit_SwordShield");
        }
        else
        {
            animator.SetInteger("currentState", (int)UnitState.Beaten);
        }

        BeatenState<UnitSoul> beatenState = states[(int)UnitState.Beaten] as BeatenState<UnitSoul>;
        _lookingDirection = new Vector3(beatenState.BeatenDirectionX, 0, beatenState.BeatenDirectionZ).normalized;

        if (_rotationTolook == null)
        {
            _rotationTolook = StartCoroutine(RotationtoLook());
        }

        StartCoroutine(WaitForBeaten(0.2f));
    }

    void OnBeatenExit()
    {
        currentState = null;
    }

    IEnumerator RotationtoLook()
    {
        float accumulatedDelta = 0;
        Vector3 lookingDirection = _lookingDirection;
        Quaternion lookingDirectionAngle = Quaternion.LookRotation(lookingDirection);
        //transform.rotation = lookingDirectionAngle; 

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

    IEnumerator WaitForSwing(float term)
    {
        yield return new WaitForSeconds(term);
        currentState = null;
    }

    IEnumerator WaitForBeaten(float term)
    {
        yield return new WaitForSeconds(term);
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

    protected void SendSkillPacket()
    {
        CTS_Skill skillPacket = new CTS_Skill();
        skillPacket.skillId = (int)SKILL_TYPE.SKILL_MELEE;

        List<int> unitlist = UnitManager.Instance.GetOtherPlayerIDs();

        foreach (int otherUnit in unitlist)
        {
            skillPacket.targets.Add(new CTS_Skill.Target() { GameObjectId = otherUnit });
        }
        
        NetworkManager.Instance.Send(skillPacket.Write());
    }

    Vector3 _fixedDirection;

    private bool CheckCollions()
    {
        float cosTheta = 0;

        _fixedDirection = Vector3.zero;

        var collList = _myCharactersCollision.Values;

        foreach (var coll in collList)
        {
            UnitSoul soul = coll.gameObject.GetComponent<UnitSoul>();

            Vector3 diff = coll.transform.position - transform.position; // 나중에 고도가 생기면 수정해야 한다.

            Vector3 diffUnit = diff.normalized;

            if(soul != null)
            {
                float tempTheta = diffUnit.x * _lookingDirection.x + diffUnit.z * _lookingDirection.z;

                if (tempTheta > 0)
                {
                    if (cosTheta > 0)
                    {
                        // 충돌상황이 2개 이상이며 이 두 원충돌체쪽으로 밀고 있다.
                        // 어느쪽으로도 움직일수 없는 상황, 사이에 낀 상황
                        // 안움직이면 된다.
                        return false;
                    }
                    else
                    {
                        // 첫번째 충돌상황은 대응가능
                        // 밀고 들어갈수없으니 옆으로 미끄러지는 상황을 연출하자
                        cosTheta = tempTheta;
                        _fixedDirection = diffUnit * cosTheta;
                    }
                }
            }
        }

        return true; ;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isMyCharacter&&
            currentState != null &&
            currentState.ID == (int)UnitState.Move &&
            other.gameObject.layer == 6)
        {
            //Vector3 diff = other.transform.position - transform.position;
            //Vector3 unitDiff = diff.normalized;

            UnitSoul otherSoul = other.gameObject.GetComponent<UnitSoul>();

            if(otherSoul != null)
            {
                if(!_myCharactersCollision.ContainsKey(otherSoul.ID))
                {
                    _myCharactersCollision.Add(otherSoul.ID, other);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isMyCharacter &&
            currentState != null &&
            currentState.ID == (int)UnitState.Move &&
            other.gameObject.layer == 6)
        {
            UnitSoul otherSoul = other.gameObject.GetComponent<UnitSoul>();

            if (otherSoul != null)
            {
                if (!_myCharactersCollision.ContainsKey(otherSoul.ID))
                {
                    _myCharactersCollision.Add(otherSoul.ID, other);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isMyCharacter &&
            other.gameObject.layer == 6)
        {
            UnitSoul otherSoul = other.gameObject.GetComponent<UnitSoul>();

            if (otherSoul != null)
            { 
                _myCharactersCollision.Remove(otherSoul.ID);
            }
        }
    }
}

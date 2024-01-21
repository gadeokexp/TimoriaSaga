using System;
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
    Dictionary<int, Collider> _myCharactersCollision;

    public int ID = 0;

    private Animator animator;

    // 이동과 회전
    public Vector3 DirectionNeedToLookAt {
        get => _DirectionNeedToLookAt;
        set
        {
            if (_DirectionNeedToLookAt.x != value.x || _DirectionNeedToLookAt.z != value.z)
            {
                _DirectionNeedToLookAt = value;
            }
        }
    }

    Vector3 _DirectionNeedToLookAt;
    Vector3 _DirectionLookingAt = Vector3.zero;

    public Vector3 TargetPosition;

    UsedInput _input;

    // 대각 이동
    float _diagonalMovementDelta = 0.1f;
    float _prevDiagonalX = 0;
    float _prevDiagonalZ = 0;

    JobQueue<int> _animationChangeCommander = new JobQueue<int>();
    float _animationChangeDelta = 0;

    // 유닛 체력
    int _maxHp;
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }

    int _hp;
    public int Hp { get { return _hp; } set { _hp = value; } }

    public void SchduleAnimationChange(int animationIndex)
    {
        _animationChangeCommander.SchduleJob(ChangeAnimation, animationIndex);
    }

    public void ChangeAnimation(int animationIndex)
    {
        animator.SetInteger("currentState", animationIndex);
    }

    void Start()
    {
        StateInit(this);

        MoveState<UnitSoul> moveState = states[(int)UnitState.Move] as MoveState<UnitSoul>;
        moveState.Enter += OnMoveEnter;
        moveState.Exit += OnMoveExit;
        moveState.Update += OnMoveUpdate;

        IdleState<UnitSoul> idleState = states[(int)UnitState.Idle] as IdleState<UnitSoul>;
        idleState.Enter += OnIdleEnter;
        idleState.Enter += OnIdleExit;
        idleState.Update += OnSimpleRotateUpdate;

        HitState<UnitSoul> hitState = states[(int)UnitState.Hit] as HitState<UnitSoul>;
        hitState.Enter += OnHitEnter;
        hitState.Exit += OnHitExit;
        hitState.Update += OnSimpleRotateUpdate;

        BeatenState<UnitSoul> beatenState = states[(int)UnitState.Beaten] as BeatenState<UnitSoul>;
        beatenState.Enter += OnBeatenEnter;
        beatenState.Exit += OnBeatenExit;

        DieState<UnitSoul> dieState = states[(int)UnitState.Die] as DieState<UnitSoul>;
        dieState.Enter += OnDieEnter;
        dieState.Exit += OnDieExit;

        if (_isMyCharacter)
        {
            InputManager.Instance.OnInput += OnInput;
            _input = InputManager.Instance.GameInput;

            _myCharactersCollision = new Dictionary<int, Collider>();
        }

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        _animationChangeDelta += Time.deltaTime;

        if(_animationChangeDelta > 0.01f && _animationChangeCommander.GetCount() > 0)
        {
            _animationChangeDelta = 0;
            _animationChangeCommander.Excute();
        }

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
            ChangeState((int)UnitState.Hit);
        }
        else if((input.XInput != 0f || input.ZInput != 0f))
        {
            // 입력 대응 2순위
            // 이동키가 눌렸고 현재 이동상태가  아니면 이동상태로 전환한다.
            ChangeState((int)UnitState.Move);
        }
        else
        {
            // 아무 키도 안눌렸으면 아이들 상태로 전환한다.
            // 이미 아이들 상태이면 전환되지 않는다.
            ChangeState((int)UnitState.Idle);
        }
    }

    void OnMoveEnter()
    {
        SchduleAnimationChange((int)UnitState.Move);

        if (_input != null)
        {
            _DirectionNeedToLookAt = new Vector3(_input.XInput, 0, _input.ZInput).normalized;
        }
    }

    void OnMoveExit()
    {
        if (!_isMyCharacter)
        {
            StartCoroutine(ClearPosition());
        }

        currentState = null;
    }

    float _moveInterval = 1f;
    Vector3 _moveRoute = Vector3.zero;
    Vector3 _moveStart = Vector3.zero;

    void OnSimpleRotateUpdate()
    {
        RotationtoLook();
    }

    void OnMoveUpdate()
    {
        float deltaTime = Time.deltaTime;

        if (_isMyCharacter)
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
                _DirectionNeedToLookAt = new Vector3(_input.XInput, 0, _input.ZInput).normalized;
            }
            //float conTheta = _CollisionDirection.x * _lookingDirection.x + _CollisionDirection.z * _lookingDirection.z;

            if(CheckCollions())
            {
                // 키보드 이동
                //transform.position += (_lookingDirection - _CollisionDirection * conTheta) * deltaTime * 3;
                transform.position += (_DirectionNeedToLookAt - _fixedDirection) * deltaTime * 3;
            }

            // 이동 보고
            if (_moveInterval > 1f || _input.DirectionChanged)
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

            if (_moveInterval < 1f)
            {
                transform.position = Vector3.Lerp(_moveStart, _moveRoute, _moveInterval);
            }
            else
            {
                transform.position = _moveRoute;
            }
        }

        // 회전 처리
        RotationtoLook();
    }

    void OnIdleEnter()
    {
        if (_diagonalMovementDelta < 0.1f)
        {
            _DirectionNeedToLookAt = new Vector3(_prevDiagonalX, 0, _prevDiagonalZ).normalized;
            _diagonalMovementDelta = 0.1f;
        }

        if(_isMyCharacter)
        {
            _myCharactersCollision.Clear();
            SendIdlePacket();
        }

        SchduleAnimationChange((int)UnitState.Idle);
    }

    void OnIdleExit()
    {
        Debug.Log("아이들 나감");
    }

    void OnHitEnter()
    {
        // 임시
        if (_animationChangeDelta < 0.001f && !IsMyCharacter)
            Debug.Log("아이들 상태변환중 너무 갑작스럽게 히트가 상태가 되었음");

        if(_isMyCharacter)
        {
            SendSkillPacket();
        }

        StartCoroutine(WaitSwing(0.6f));

        int currentMotion = animator.GetInteger("currentState");

        if (currentMotion == 2)
        {
            SchduleAnimationChange(201);
        }
        else
        {
            SchduleAnimationChange((int)UnitState.Hit);
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
            SchduleAnimationChange((int)UnitState.Beaten);
        }

        BeatenState<UnitSoul> beatenState = states[(int)UnitState.Beaten] as BeatenState<UnitSoul>;
        _DirectionNeedToLookAt = new Vector3(beatenState.BeatenDirectionX, 0, beatenState.BeatenDirectionZ).normalized;

        // 히트 이펙트
        GameObject hitObject = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.EffectHit);
        hitObject.transform.position = transform.position + Vector3.up + _DirectionNeedToLookAt * 0.4f;

        RotationtoLook();

        StartCoroutine(WaitBeaten(0.2f));
    }

    void OnBeatenExit()
    {
        currentState = null;
    }

    void OnDieEnter()
    {
        SchduleAnimationChange((int)UnitState.Die);

        if (IsMyCharacter)
        {
            // 내 유닛이 죽었을 경우 게임오버 팝업 실행
            StartCoroutine(WaitDieRoutine(1.5f));
        }
        else
        {
            // 각 클라이언트들은 각자 유닛간 충돌 관계를 관리하는 부분이 있다.
            // 원래 죽으면 컬리전 Exit 핸들러에서 이를 처리해줘야 하는데
            // 컬리전이 단순히 비활성화 될때 종종 처리가안된다.
            // 그래서 어떤 유닛이 죽으면 각 클라이언트들은 죽은 유닛의 충돌 관계를 손수 정리해줄 필요가 있었다.
            UnitManager.Instance.PlayerUnitSoul.RemoveCollisionRelationshipWith(ID);
        }   

        CapsuleCollider[] colliders = gameObject.GetComponents<CapsuleCollider>();

        foreach (CapsuleCollider collider in colliders)
        {
            collider.enabled = false;
        }

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = true;

        // 히트 이펙트 처리
        DieState<UnitSoul> dieState = states[(int)UnitState.Die] as DieState<UnitSoul>;
        _DirectionNeedToLookAt = new Vector3(dieState.BeatenDirectionX, 0, dieState.BeatenDirectionZ).normalized;
        GameObject hitObject = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.EffectHit);
        hitObject.transform.position = transform.position + Vector3.up + _DirectionNeedToLookAt * 0.4f;
    }

    public void RemoveCollisionRelationshipWith(int id)
    {
        if (!IsMyCharacter) return;

        if(_myCharactersCollision.ContainsKey(id))
        {
            _myCharactersCollision.Remove(id);
        }
    }

    void OnDieExit()
    {
    }

    float _accumulatedDelta = 1.0f;
    Quaternion lookingDirectionAngle = Quaternion.identity;

    void RotationtoLook()
    {
        //Vector3 lookingDirection = _DirectionNeedToLookAt;
        // = Quaternion.LookRotation(lookingDirection);

        // 회전 방향이 변했을 경우 리셋
        if (_DirectionLookingAt != _DirectionNeedToLookAt)
        {
            _DirectionLookingAt = _DirectionNeedToLookAt;
            lookingDirectionAngle = Quaternion.LookRotation(_DirectionNeedToLookAt);
            _accumulatedDelta = 0;
        }

        if (_accumulatedDelta < 1.0f)
        {
            _accumulatedDelta += Time.deltaTime * 2;

            // 회전이 끝났을 경우 정리
            if (_accumulatedDelta >= 0.98f)
            {
                transform.rotation = lookingDirectionAngle;
                _accumulatedDelta = 1.0f;
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, lookingDirectionAngle, _accumulatedDelta);
            }
        }
    }

    public void OnRevive(STC_Revive revivePacket)
    {
        // 유닛 애니메이션 아이들로 강제 전환
        animator.SetInteger("currentState", 0);
        animator.Play("Idle_SwordShield");

        transform.position = new Vector3(revivePacket.positionX, revivePacket.positionY, revivePacket.positionZ);
        transform.forward = new Vector3(0, 0, -1);

        _hp = _maxHp;

        CapsuleCollider[] colliders = gameObject.GetComponents<CapsuleCollider>();

        foreach (CapsuleCollider collider in colliders)
        {
            collider.enabled = true;
        }

        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;

        currentState = null;
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

    IEnumerator WaitSwing(float term)
    {
        yield return new WaitForSeconds(term);
        currentState = null;
    }

    IEnumerator WaitBeaten(float term)
    {
        yield return new WaitForSeconds(term);
        currentState = null;
    }

    IEnumerator WaitDieRoutine(float term)
    {
        yield return new WaitForSeconds(term);

        SubUIManageField fieldUI = UIManager.Instance.CurrentUISubManager as SubUIManageField;

        if (fieldUI != null)
        {
            fieldUI.ShowGameOverPopup(true);
        }
    }

    protected void SendMovePacket()
    {
        CTS_Move movePacket = new CTS_Move();
        movePacket.positionX = transform.position.x;
        movePacket.positionY = transform.position.y;
        movePacket.positionZ = transform.position.z;
        movePacket.directionX = _DirectionNeedToLookAt.x;
        movePacket.directionZ = _DirectionNeedToLookAt.z;
        movePacket.timeStamp = 33333333;

        NetworkManager.Instance.Send(movePacket.Write());
    }

    protected void SendIdlePacket()
    {
        CTS_Idle idlePacket = new CTS_Idle();
        idlePacket.directionX = _DirectionNeedToLookAt.x;
        idlePacket.directionZ = _DirectionNeedToLookAt.z;
        idlePacket.timeStamp = 33333333;

        NetworkManager.Instance.Send(idlePacket.Write());
    }

    protected void SendSkillPacket()
    {
        CTS_Skill skillPacket = new CTS_Skill();
        skillPacket.skillId = (int)SKILL_TYPE.SKILL_MELEE;
        skillPacket.directionX = _DirectionNeedToLookAt.x;
        skillPacket.directionZ = _DirectionNeedToLookAt.z;

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
                float tempTheta = diffUnit.x * _DirectionNeedToLookAt.x + diffUnit.z * _DirectionNeedToLookAt.z;

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
        if(_isMyCharacter&&
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
        if (_isMyCharacter &&
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
        if (_isMyCharacter &&
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

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class UnitSoul : MonoBehaviour
{
    [SerializeField]
    public bool isMyCharacter = true;

    Vector3 _lookingDirection;
    Coroutine _rotationTolook;

    void Start()
    { 
        if(isMyCharacter )
        {
            InputManager.Instance.OnMove += OnMove;
        }
    }

    void OnMove(float XInput, float ZInput)
    {
        // 사용자 인풋이나, 패킷에 수신등의 이벤트등으로 콜된다.
        _lookingDirection = new Vector3(XInput, 0, ZInput).normalized;

        transform.position += _lookingDirection * Time.deltaTime * 3;

        if (_rotationTolook == null)
        {
            _rotationTolook = StartCoroutine(RotationtoLook());
        }
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

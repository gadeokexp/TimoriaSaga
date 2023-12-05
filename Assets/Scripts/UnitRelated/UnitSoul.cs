using System.Collections;
using System.Collections.Generic;
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
        while (true)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_lookingDirection), Time.deltaTime * 4);
            yield return null;
        }
    }
}

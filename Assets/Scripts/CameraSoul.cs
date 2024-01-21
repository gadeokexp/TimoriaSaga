using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSoul : MonoBehaviour
{
    public Transform Target { get => _target; set { _target = value; } }
    private Transform _target;

    private void Start()
    {
        UnitManager.Instance.SetCameraSlot = SetCameraTarget;
        _target = GameInstance.Instance.transform;
    }
    void Update()
    {
        transform.position = new Vector3(_target.position.x, _target.position.y + 10f, _target.position.z - 10f);
        transform.LookAt(_target.position);
    }

    void SetCameraTarget()
    {
        if(UnitManager.Instance.PlayerUnit != null)
        {
            Target = UnitManager.Instance.PlayerUnit.transform;
        }
        else
        {
            Target = GameInstance.Instance.transform;
        }
    }
}

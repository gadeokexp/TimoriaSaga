using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSoul : MonoBehaviour
{
    public Transform Target => target;
    private Transform target;

    private void Start()
    {
        target = UnitManager.Instance.GetCameraTarget();
    }

    void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y + 10f, target.position.z - 10f);
        transform.LookAt(target.position);
    }
}

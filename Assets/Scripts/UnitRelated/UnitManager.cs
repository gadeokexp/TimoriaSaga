using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    public UnitManager()
    {
        GameObject player = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.Player);

        // 플레이어 위치 설정
        player.transform.position = Vector3.up * 5;

        // 플레이어 컴포넌트 설정
        Rigidbody rigid = player.AddComponent<Rigidbody>();
        player.AddComponent<UnitSoul>();
        CapsuleCollider cc = player.AddComponent<CapsuleCollider>();
        cc.height = 1.88f;
        cc.radius = 0.5f;
        cc.center = new Vector3(0, 0.9f, 0);

        // 부딪치고 회전하는거 방지
        rigid.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }
}

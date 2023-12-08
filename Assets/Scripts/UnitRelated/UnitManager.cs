using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    GameObject player = null;
    GameObject player2 = null;

    public UnitManager()
    {
        // 첫번째 플레이어
        player = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.Player);

        player.transform.position = Vector3.up * 5;

        // 플레이어 컴포넌트 설정
        Rigidbody rigid = player.AddComponent<Rigidbody>();
        player.AddComponent<UnitSoul>();
        CapsuleCollider cc = player.AddComponent<CapsuleCollider>();
        cc.height = 1.88f;
        cc.radius = 0.5f;
        cc.center = new Vector3(0, 0.9f, 0);

        // 부딪치고 회전하는거 방지
        rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;


        // 두번째 플레이어
        player2 = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.Player2);

        // 플레이어 위치 설정
        player2.transform.position = Vector3.right * 5 + Vector3.up * 2;
        
        rigid = player2.AddComponent<Rigidbody>();
        player2.AddComponent<UnitSoul>();
        cc = player2.AddComponent<CapsuleCollider>();
        cc.height = 1.88f;
        cc.radius = 0.5f;
        cc.center = new Vector3(0, 0.9f, 0);

        // 부딪치고 회전하는거 방지
        rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public Transform GetCameraTarget()
    {
        return player.transform;
    }
}

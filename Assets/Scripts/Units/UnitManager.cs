using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    GameObject _player = null;
    Dictionary<int, GameObject> _otherPlayers = new Dictionary<int, GameObject>();

    public UnitManager()
    {
        //SpawnUnit(true, 0, 5, 0);
    }

    public void SpawnUnit(bool isMyCharacter, int Id, float x, float y, float z)
    {
        // 첫번째 플레이어
        GameObject player = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.Player);

        //player.transform.position = Vector3.up * 5;
        player.transform.position = new Vector3(x, y, z);

        // 리지드 바디
        Rigidbody rigid = player.AddComponent<Rigidbody>();

        // 소울
        UnitSoul soul = player.AddComponent<UnitSoul>();
        soul.isMyCharacter = isMyCharacter;        

        // 충돌체
        CapsuleCollider cc = player.AddComponent<CapsuleCollider>();
        cc.height = 1.88f;
        cc.radius = 0.5f;
        cc.center = new Vector3(0, 0.9f, 0);

        // 부딪치고 회전하는거 방지
        rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (isMyCharacter)
        {
            _player = player;
        }
        else
        {
            _otherPlayers.Add(Id, player);
        }

    }

    public Transform GetCameraTarget()
    {
        return _player != null? _player.transform : GameInstance.Instance.transform;
    }

    public void DespawnUnit(int id)
    {
        GameObject unit;

        if(_otherPlayers.TryGetValue(id, out unit))
        {
            _otherPlayers.Remove(id);
            ResourceManager.Instance.DespawnObject(unit.gameObject);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UnitManager : Singleton<UnitManager>
{
    public GameObject Player => _player;
    GameObject _player = null;
    public int _playerID;
    public int PlayerID => _playerID;

    public Action SetCamreaSlot;

    Dictionary<int, GameObject> _otherPlayers = new Dictionary<int, GameObject>();

    public UnitManager()
    {
        //SpawnUnit(true, 0, 5, 0);
    }

    public void SpawnUnit(bool isMyCharacter,int Id, Vector3 position, Vector3 rotation, int maxHp, int Hp)
    {
        // 첫번째 플레이어
        GameObject player = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.Player);

        //player.transform.position = Vector3.up * 5;
        player.transform.position = position;
        player.transform.rotation = Quaternion.LookRotation(rotation);

        // 리지드 바디
        Rigidbody rigid = player.AddComponent<Rigidbody>();

        // 소울
        UnitSoul soul = player.AddComponent<UnitSoul>();
        soul.IsMyCharacter = isMyCharacter;
        soul.ID = Id;
        soul.MaxHp = maxHp;
        soul.Hp = Hp;

        // 충돌체
        CapsuleCollider cc = player.AddComponent<CapsuleCollider>();
        cc.height = 1.88f;
        cc.radius = 0.65f;
        cc.center = new Vector3(0, 0.9f, 0);

        // 유닛끼리만 사용하는 트리거 충돌체
        CapsuleCollider uc = player.AddComponent<CapsuleCollider>();
        uc.height = 1.88f;
        uc.radius = 0.65f;
        uc.center = new Vector3(0, 0.9f, 0);
        uc.isTrigger = true;
        uc.includeLayers = 1 << 6; // Unit 레이어 설정

        // 부딪치고 회전하는거 방지
        rigid.constraints |= RigidbodyConstraints.FreezeRotation;

        if (isMyCharacter)
        {
            _player = player;
            _playerID = Id;
            if (SetCamreaSlot != null) SetCamreaSlot.Invoke();
        }
        else
        {
            _otherPlayers.Add(Id, player);
        }
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

    public GameObject SearchById(int id)
    {
        GameObject obj = null;
        _otherPlayers.TryGetValue(id, out obj);
        return obj;
    }

    public List<GameObject> GetOtherPlayers()
    {
        return _otherPlayers.Values.ToList<GameObject>();
    }

    public List<int> GetOtherPlayerIDs()
    {
        return _otherPlayers.Keys.ToList<int>();
    }
}

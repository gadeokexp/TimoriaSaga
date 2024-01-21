using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UnitManager : Singleton<UnitManager>
{
    public GameObject PlayerUnit => _playerUnit;
    GameObject _playerUnit = null;

    public UnitSoul PlayerUnitSoul => _playerUnitSoul;
    UnitSoul _playerUnitSoul = null;

    int _playerID;
    public int PlayerID => _playerID;

    public Action SetCameraSlot;

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

        // 소울
        UnitSoul us = player.AddComponent<UnitSoul>();
        us.IsMyCharacter = isMyCharacter;
        us.ID = Id;
        us.MaxHp = maxHp;
        us.Hp = Hp;

        if(isMyCharacter)
        {
            _playerUnitSoul = us;
        }

        // 부딪치고 회전하는거 방지
        rigid.constraints |= RigidbodyConstraints.FreezeRotation;

        if (isMyCharacter)
        {
            _playerUnit = player;

            _playerID = Id;
            if (SetCameraSlot != null) SetCameraSlot.Invoke();
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
        // 주의 : 클라이언트의 플레이어 유닛은 반환하지 않는다

        // 이동처리를 할때 플레이어 유닛은 사용자 인풋에 따라 미리 이동하고 있다.
        // 패킷 받아 처리할 필요가 없는 것

        // 이 때문에 다른 유닛만 처리해주기 위해 이렇게 처리함

        GameObject obj = null;

        _otherPlayers.TryGetValue(id, out obj);

        return obj;
    }

    public UnitSoul SearchSoulById(int id)
    {
        // 주의 : 클라이언트의 플레이어 유닛 소울도 반환한다

        UnitSoul unitSoul = null;

        if(id == _playerID)
        {
            unitSoul = _playerUnitSoul;
        }
        else
        {
            GameObject unitObject;

            _otherPlayers.TryGetValue(id, out unitObject);

            if (unitObject != null)
            {
                unitSoul = unitObject.GetComponent<UnitSoul>();
            }
        }
        
        return unitSoul;
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

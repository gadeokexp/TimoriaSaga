using System;
using TimoriaSagaNetworkLibrary;
using Unity.VisualScripting;
using UnityEngine;

internal class PacketHandler
{
    public static void STC_EnterFieldHandler(PacketSession session, IPacket packet)
    {
        STC_EnterField enterPacket = packet as STC_EnterField;

        Vector3 position = new Vector3(enterPacket.positionX, enterPacket.positionY, enterPacket.positionZ);
        UnitManager.Instance.SpawnUnit(true, enterPacket.GameObjectId, position, Vector3.back);
    }

    public static void STC_LeaveFieldHandler(PacketSession session, IPacket packet)
    {
    }

    public static void STC_SpawnHandler(PacketSession session, IPacket packet)
    {
    }

    public static void STC_UnitSpawnHandler(PacketSession session, IPacket packet)
    {
        // 사용자의 MyUnit을 제외한 모든 객체는 결국 이 Spawnhandler를 통해 생성된다
        // 다른 사용자의 Unit도, Monster도, 화살도, 등등 모든 것들이 다.
        // 그리고 이런것들은 다 각자의 유니크한 ID를 가지고 있다.

        STC_UnitSpawn spawnPacket = packet as STC_UnitSpawn;

        foreach (STC_UnitSpawn.GameObject objectInfo in spawnPacket.gameObjects)
        {
            Vector3 position = new Vector3(objectInfo.positionX, objectInfo.positionY, objectInfo.positionZ);
            Vector3 rotation = new Vector3(objectInfo.directionX, 0, objectInfo.directionZ);

            UnitManager.Instance.SpawnUnit(false, objectInfo.GameObjectId, position, rotation);
        }
    }

    public static void STC_DieHandler(PacketSession session, IPacket packet)
    {
    }

    public static void STC_ChangeHpHandler(PacketSession session, IPacket packet)
    {
    }

    public static void STC_DespawnHandler(PacketSession session, IPacket packet)
    {
        STC_Despawn despawnPacket = packet as STC_Despawn;

        foreach (STC_Despawn.GameObjectId gameObjectId in despawnPacket.gameObjectIds)
        {
            UnitManager.Instance.DespawnUnit(gameObjectId.ID);
        }
    }

    public static void STC_DespawnProjectileAtHandler(PacketSession session, IPacket packet)
    {
    }

    public static void STC_FixPositionHandler(PacketSession session, IPacket packet)
    {
    }

    public static void STC_MoveHandler(PacketSession session, IPacket packet)
    {
        STC_Move movePacket = packet as STC_Move;

        GameObject unit = UnitManager.Instance.SearchById(movePacket.GameObjectId);
        if (unit == null) return;

        UnitSoul unitSoul = unit.GetComponent<UnitSoul>();
        if (unitSoul == null) return;

        unitSoul.TargetPosition = new Vector3(movePacket.positionX, movePacket.positionY, movePacket.positionZ);
        unitSoul.DirectionNeedToLookAt = new Vector3(movePacket.directionX, 0, movePacket.directionZ);
        unitSoul.ChangeState((int)UnitState.Move);
    }

    public static void STC_IdleHandler(PacketSession session, IPacket packet)
    {
        STC_Idle idlePacket = packet as STC_Idle;

        GameObject unit = UnitManager.Instance.SearchById(idlePacket.GameObjectId);
        if (unit == null) return;

        UnitSoul unitSoul = unit.GetComponent<UnitSoul>();
        if (unitSoul == null) return;

        unitSoul.DirectionNeedToLookAt = new Vector3(idlePacket.directionX, 0, idlePacket.directionZ);
        unitSoul.ChangeState((int)UnitState.Idle);
    }

    public static void STC_TurnHandler(PacketSession session, IPacket packet)
    {
    }

    public static void STC_SkillHandler(PacketSession session, IPacket packet)
    {
        STC_Skill skillPacket = packet as STC_Skill;

        if (skillPacket.GameObjectId != UnitManager.Instance.PlayerID)
        {
            GameObject unit = UnitManager.Instance.SearchById(skillPacket.GameObjectId);   

            UnitSoul unitSoul = unit.GetComponent<UnitSoul>();
            if (unitSoul == null) return;

            unitSoul.DirectionNeedToLookAt = new Vector3(skillPacket.directionX, 0, skillPacket.directionZ);
            unitSoul.ChangeState((int)UnitState.Hit);
        }        
    }

    public static void STC_BeatenHandler(PacketSession session, IPacket packet)
    {
        STC_Beaten beatenPacket = packet as STC_Beaten;

        foreach(var target in beatenPacket.targets)
        {
            UnitSoul unitSoul;

            if (target.GameObjectId == UnitManager.Instance.PlayerID)
            {
                unitSoul = UnitManager.Instance.Player.GetComponent<UnitSoul>();
                if (unitSoul == null) continue;
            }
            else
            {
                GameObject unit = UnitManager.Instance.SearchById(target.GameObjectId);

                unitSoul = unit.GetComponent<UnitSoul>();
                if (unitSoul == null) continue;
            }

            BeatenState<UnitSoul>  unitBeatenState = unitSoul.States[(int)UnitState.Beaten] as BeatenState<UnitSoul>;
            unitBeatenState.AttackkerID = beatenPacket.GameObjectId;
            unitBeatenState.BeatenDirectionX = -beatenPacket.directionX;
            unitBeatenState.BeatenDirectionZ = -beatenPacket.directionZ;
            unitBeatenState.SkillID = beatenPacket.skillId;

            unitSoul.ChangeState((int)UnitState.Beaten);
        }
    }

    public static void STC_ConnectedHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("서버로부터 STC_Connect 패킷 받음");

        // 접속이 되면 로그인 요청을 하자
        CTS_RequestLogin loginPacket = new CTS_RequestLogin();

        loginPacket.UniqueId = DataManager.Instance.AccountName;
        loginPacket.AccountPassword = DataManager.Instance.Password;

        // 일단 이걸 ID로 쓰자
        NetworkManager.Instance.UniqueId = loginPacket.UniqueId;
        NetworkManager.Instance.Send(loginPacket.Write());
    }

    public static void STC_PermitLoginHandler(PacketSession session, IPacket packet)
    {
        // 서버에서 로그인이 되었다는 퍼밋메시지이다
        // 로비 화면에서 디스플레이해줄 캐릭터 목록도 들어있다.

        // 캐스팅이 확실히 가능하다는 보장이 있으면 ()를 이용한 캐스팅이 빠르다
        STC_PermitLogin loginPacket = (STC_PermitLogin)packet;

        // 일단 디스플레이 해줄 화면이 없으니 플레이어가 있는지 없는지 구분만하자
        if (loginPacket.units == null || loginPacket.units.Count == 0)
        {
            // 없으면 일단 자동으로 하나 만들자
            //CTS_CreatNewUnit createUnit = new CTS_CreatNewUnit();
            //createUnit.UnitName = $"player_{(int)UnityEngine.Random.Range(1, 100000)}";
            //NetworkManager.Instance.Send(createUnit.Write());
        }
        else
        {
            STC_PermitLogin.Unit unitInfo = loginPacket.units[0];
            CTS_EnterField enterPacket = new CTS_EnterField();
            enterPacket.UnitName = unitInfo.UnitName;
            DataManager.Instance.UnitName = unitInfo.UnitName;

            NetworkManager.Instance.Send(enterPacket.Write());

            GameInstance.Instance.StartGame();

        }
    }

    public static void STC_DenyLoginHandler(PacketSession session, IPacket packet)
    {
    }

    public static void STC_ResponseCreatNewUnitHandler(PacketSession session, IPacket packet)
    {
    }

}

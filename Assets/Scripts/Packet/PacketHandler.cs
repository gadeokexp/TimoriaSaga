using System;
using TimoriaSagaNetworkLibrary;
using UnityEngine;

internal class PacketHandler
{
    public static void STC_EnterGameHandler(PacketSession session, IPacket packet)
    {
        //STC_Chat chatPacket = packet as STC_Chat;
        //ClientSideSession clientSideSession = session as ClientSideSession;
    }

    public static void STC_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        //STC_Chat chatPacket = packet as STC_Chat;
        //ClientSideSession clientSideSession = session as ClientSideSession;
    }

    public static void STC_SpawnHandler(PacketSession session, IPacket packet)
    {
        //STC_Chat chatPacket = packet as STC_Chat;
        //ClientSideSession clientSideSession = session as ClientSideSession;
    }

    public static void STC_UnitSpawnHandler(PacketSession session, IPacket packet)
    {
        //STC_Chat chatPacket = packet as STC_Chat;
        //ClientSideSession clientSideSession = session as ClientSideSession;
    }

    public static void STC_DieHandler(PacketSession session, IPacket packet)
    {
        //STC_Chat chatPacket = packet as STC_Chat;
        //ClientSideSession clientSideSession = session as ClientSideSession;
    }

    public static void STC_ChangeHpHandler(PacketSession session, IPacket packet)
    {
        //STC_Chat chatPacket = packet as STC_Chat;
        //ClientSideSession clientSideSession = session as ClientSideSession;
    }

    public static void STC_DespawnHandler(PacketSession session, IPacket packet)
    {
        //STC_Chat chatPacket = packet as STC_Chat;
        //ClientSideSession clientSideSession = session as ClientSideSession;
    }

    public static void STC_DespawnProjectileAtHandler(PacketSession session, IPacket packet)
    {
        //STC_Chat chatPacket = packet as STC_Chat;
        //ClientSideSession clientSideSession = session as ClientSideSession;
    }

    public static void STC_FixPositionHandler(PacketSession session, IPacket packet)
    {
        //STC_Chat chatPacket = packet as STC_Chat;
        //ClientSideSession clientSideSession = session as ClientSideSession;
    }

    public static void STC_MoveHandler(PacketSession session, IPacket packet)
    {
        //STC_Chat chatPacket = packet as STC_Chat;
        //ClientSideSession clientSideSession = session as ClientSideSession;
    }

    public static void STC_TurnHandler(PacketSession session, IPacket packet)
    {
        //STC_Chat chatPacket = packet as STC_Chat;
        //ClientSideSession clientSideSession = session as ClientSideSession;
    }

    public static void STC_SkillHandler(PacketSession session, IPacket packet)
    {
        //STC_Chat chatPacket = packet as STC_Chat;
        //ClientSideSession clientSideSession = session as ClientSideSession;
    }

    public static void STC_ConnectedHandler(PacketSession session, IPacket packet)
    {
        Debug.Log("서버로부터 STC_Connect 패킷 받음");

        // 접속이 되면 로그인 요청을 하자
        CTS_RequestLogin loginPacket = new CTS_RequestLogin();

        // 장비에 유니크한 스트링을 유니티에서 만들어주는 속성을 이용한다.
        // 단지 이때는 로컬에 있는 모든 클라이언트가 같은 값을 뱉을 수 있기에 주의 필요
        //SystemInfo.deviceUniqueIdentifier;
        loginPacket.UniqueId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
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
            // 이미 만든 플레이어가 응답으로 왔을때는 첫번째 캐릭터로 로그인(사실 하나밖에 없다)
            STC_PermitLogin.Unit unitInfo = loginPacket.units[0];
            CTS_EnterGame enterPacket = new CTS_EnterGame();
            enterPacket.UnitName = unitInfo.UnitName;
            NetworkManager.Instance.Send(enterPacket.Write());
        }
    }

    public static void STC_ResponseCreatNewUnitHandler(PacketSession session, IPacket packet)
    {
        //STC_Chat chatPacket = packet as STC_Chat;
        //ClientSideSession clientSideSession = session as ClientSideSession;
    }

}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimoriaSagaDummyClient;
using TimoriaSagaNetworkLibrary;

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
        STC_Connected chatPacket = packet as STC_Connected;
        ClientSideSession clientSideSession = session as ClientSideSession;
        Console.WriteLine("서버로부터 STC_Connect 패킷 받음");
    }

    public static void STC_PermitLoginHandler(PacketSession session, IPacket packet)
    {
        //STC_Chat chatPacket = packet as STC_Chat;
        //ClientSideSession clientSideSession = session as ClientSideSession;
    }

    public static void STC_ResponseCreatNewUnitHandler(PacketSession session, IPacket packet)
    {
        //STC_Chat chatPacket = packet as STC_Chat;
        //ClientSideSession clientSideSession = session as ClientSideSession;
    }

}

using System;
using System.Collections.Generic;
using TimoriaSagaNetworkLibrary;

// version 1.0

public class PacketManager
{
    #region 싱글톤 구현부
    static PacketManager mcInstance = new PacketManager();

    public static PacketManager Instance { get { return mcInstance; } }

    PacketManager()
    {
        Register();
    }
    #endregion

    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> maMakePacketJob
        = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> maHandler
        = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public Action<PacketSession, IPacket> CustomHandler { get; set; }

    public void Register()
    {
        maMakePacketJob.Add((ushort)PacketID.STC_EnterGame, MakePacket<STC_EnterGame>);
        maHandler.Add((ushort)PacketID.STC_EnterGame, PacketHandler.STC_EnterGameHandler);
        maMakePacketJob.Add((ushort)PacketID.STC_LeaveGame, MakePacket<STC_LeaveGame>);
        maHandler.Add((ushort)PacketID.STC_LeaveGame, PacketHandler.STC_LeaveGameHandler);
        maMakePacketJob.Add((ushort)PacketID.STC_Spawn, MakePacket<STC_Spawn>);
        maHandler.Add((ushort)PacketID.STC_Spawn, PacketHandler.STC_SpawnHandler);
        maMakePacketJob.Add((ushort)PacketID.STC_UnitSpawn, MakePacket<STC_UnitSpawn>);
        maHandler.Add((ushort)PacketID.STC_UnitSpawn, PacketHandler.STC_UnitSpawnHandler);
        maMakePacketJob.Add((ushort)PacketID.STC_Despawn, MakePacket<STC_Despawn>);
        maHandler.Add((ushort)PacketID.STC_Despawn, PacketHandler.STC_DespawnHandler);
        maMakePacketJob.Add((ushort)PacketID.STC_DespawnProjectileAt, MakePacket<STC_DespawnProjectileAt>);
        maHandler.Add((ushort)PacketID.STC_DespawnProjectileAt, PacketHandler.STC_DespawnProjectileAtHandler);
        maMakePacketJob.Add((ushort)PacketID.STC_Move, MakePacket<STC_Move>);
        maHandler.Add((ushort)PacketID.STC_Move, PacketHandler.STC_MoveHandler);
        maMakePacketJob.Add((ushort)PacketID.STC_Turn, MakePacket<STC_Turn>);
        maHandler.Add((ushort)PacketID.STC_Turn, PacketHandler.STC_TurnHandler);
        maMakePacketJob.Add((ushort)PacketID.STC_FixPosition, MakePacket<STC_FixPosition>);
        maHandler.Add((ushort)PacketID.STC_FixPosition, PacketHandler.STC_FixPositionHandler);
        maMakePacketJob.Add((ushort)PacketID.STC_Skill, MakePacket<STC_Skill>);
        maHandler.Add((ushort)PacketID.STC_Skill, PacketHandler.STC_SkillHandler);
        maMakePacketJob.Add((ushort)PacketID.STC_ChangeHp, MakePacket<STC_ChangeHp>);
        maHandler.Add((ushort)PacketID.STC_ChangeHp, PacketHandler.STC_ChangeHpHandler);
        maMakePacketJob.Add((ushort)PacketID.STC_Die, MakePacket<STC_Die>);
        maHandler.Add((ushort)PacketID.STC_Die, PacketHandler.STC_DieHandler);
        maMakePacketJob.Add((ushort)PacketID.STC_Connected, MakePacket<STC_Connected>);
        maHandler.Add((ushort)PacketID.STC_Connected, PacketHandler.STC_ConnectedHandler);
        maMakePacketJob.Add((ushort)PacketID.STC_PermitLogin, MakePacket<STC_PermitLogin>);
        maHandler.Add((ushort)PacketID.STC_PermitLogin, PacketHandler.STC_PermitLoginHandler);
        maMakePacketJob.Add((ushort)PacketID.STC_ResponseCreatNewUnit, MakePacket<STC_ResponseCreatNewUnit>);
        maHandler.Add((ushort)PacketID.STC_ResponseCreatNewUnit, PacketHandler.STC_ResponseCreatNewUnitHandler);

    }

    public void OnReceivePacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> packetTaker = null)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Func<PacketSession, ArraySegment<byte>, IPacket> packetJob = null;
        if (maMakePacketJob.TryGetValue(id, out packetJob))
        {
            IPacket packet = packetJob.Invoke(session, buffer);

            if (packetTaker != null)
            {
                packetTaker.Invoke(session, packet);
            }
            else if (CustomHandler != null)
            {
                CustomHandler.Invoke(session, packet);
            }
            else
                HandlePacket(session, packet);
        }
    }

    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        // 함수가 받는 제너릭 인자는 IPacket인터페이스를 상속받으면서 동시에 new로 생성가능한 클래스여야 한다

        T packet = new T();
        packet.Read(buffer);
        return packet;
    }

    public void HandlePacket(PacketSession session, IPacket packet)
    {
        Action<PacketSession, IPacket> action = null;
        if (maHandler.TryGetValue(packet.Protocol, out action))
        {
            action.Invoke(session, packet);
        }
    }
}
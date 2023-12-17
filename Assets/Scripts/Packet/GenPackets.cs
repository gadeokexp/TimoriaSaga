using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TimoriaSagaNetworkLibrary;

public enum PacketID
{
    CTS_EnterField = 1,
	STC_EnterField = 2,
	STC_LeaveField = 3,
	STC_Spawn = 4,
	STC_UnitSpawn = 5,
	STC_Despawn = 6,
	STC_DespawnProjectileAt = 7,
	CTS_Move = 8,
	STC_Move = 9,
	CTS_Idle = 10,
	STC_Idle = 11,
	STC_Turn = 12,
	STC_FixPosition = 13,
	CTS_Skill = 14,
	STC_Skill = 15,
	STC_ChangeHp = 16,
	STC_Die = 17,
	CTS_Die = 18,
	STC_Connected = 19,
	CTS_RequestLogin = 20,
	STC_PermitLogin = 21,
	CTS_CreatNewUnit = 22,
	STC_ResponseCreatNewUnit = 23,
	
}

public enum PlayerGameMode{
    GAMEMOVE_LOGIN = 0,
    GAMEMOVE_LOBBY = 1,
    GAMEMOVE_WORLDMAP = 2,
    GAMEMOVE_FIELD = 3,
}

public enum GameObjectType{
    NONE = 0,
    UNIT = 1,
    MONSTER = 2,
    PROJECTILE = 3,
}

public enum OBJECT_DIRECTION
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
}

public enum CREATURE_STATE
{
    IDLE,
    MOVING,
    SKILL,
    DEAD,
}

public enum SKILL_TYPE
{
    SKILL_NONE,
    SKILL_AUTO,
    SKILL_PROJECTILE,
}

public struct StatusInfo
{
    float speed;
    int level;
    int attack;
    int defence;
    int dexterity;
    int agility;
    int intelligence;
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}

public class CTS_EnterField : IPacket
{
    public string UnitName;
    public ushort Protocol { get { return (ushort)PacketID.CTS_EnterField; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        ushort UnitNameLength = BitConverter.ToUInt16(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(ushort);
		this.UnitName = Encoding.Unicode.GetString(readSpan.Slice(count, UnitNameLength));
		count += UnitNameLength;
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;
        packetLength += 2 + (ushort)Encoding.Unicode.GetByteCount(this.UnitName);
        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.CTS_EnterField);
        count += sizeof(ushort);

        ushort UnitNameLength = (ushort)Encoding.Unicode.GetBytes(this.UnitName, 0, this.UnitName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), UnitNameLength);
		count += sizeof(ushort);
		count += UnitNameLength;
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class STC_EnterField : IPacket
{
    public int GameObjectId;
	public string GameObjectName;
	public float positionX;
	public float positionY;
	public float positionZ;
	public int maxHp;
	public float speed;
	public int state;
	public int hp;
	public int level;
	public int strength;
	public int dexterity;
	public int resistance;
	public int proficiency;
	public int vitality;
    public ushort Protocol { get { return (ushort)PacketID.STC_EnterField; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        this.GameObjectId = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		ushort GameObjectNameLength = BitConverter.ToUInt16(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(ushort);
		this.GameObjectName = Encoding.Unicode.GetString(readSpan.Slice(count, GameObjectNameLength));
		count += GameObjectNameLength;
		
		this.positionX = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(float);
		
		this.positionY = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(float);
		
		this.positionZ = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(float);
		
		this.maxHp = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.speed = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(float);
		
		this.state = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.hp = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.level = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.strength = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.dexterity = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.resistance = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.proficiency = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.vitality = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;
        packetLength += sizeof(int);
        packetLength += 2 + (ushort)Encoding.Unicode.GetByteCount(this.GameObjectName);
        packetLength += sizeof(float);
        packetLength += sizeof(float);
        packetLength += sizeof(float);
        packetLength += sizeof(int);
        packetLength += sizeof(float);
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.STC_EnterField);
        count += sizeof(ushort);

        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.GameObjectId);
		count += sizeof(int);
		
		ushort GameObjectNameLength = (ushort)Encoding.Unicode.GetBytes(this.GameObjectName, 0, this.GameObjectName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), GameObjectNameLength);
		count += sizeof(ushort);
		count += GameObjectNameLength;
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionX);
		count += sizeof(float);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionY);
		count += sizeof(float);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionZ);
		count += sizeof(float);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.maxHp);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.speed);
		count += sizeof(float);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.state);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.hp);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.level);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.strength);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.dexterity);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.resistance);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.proficiency);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.vitality);
		count += sizeof(int);
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class STC_LeaveField : IPacket
{
    
    public ushort Protocol { get { return (ushort)PacketID.STC_LeaveField; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;

        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.STC_LeaveField);
        count += sizeof(ushort);

        
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class STC_Spawn : IPacket
{
    
	public struct GameObject
	{
	    public int GameObjectId;
		public string GameObjectName;
		public float positionX;
		public float positionY;
		public float positionZ;
		public float speed;
		public int state;
	
	    public void Read(ReadOnlySpan<byte> readSpan, ref ushort count)
	    {
	        this.GameObjectId = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(int);
			
			ushort GameObjectNameLength = BitConverter.ToUInt16(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(ushort);
			this.GameObjectName = Encoding.Unicode.GetString(readSpan.Slice(count, GameObjectNameLength));
			count += GameObjectNameLength;
			
			this.positionX = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(float);
			
			this.positionY = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(float);
			
			this.positionZ = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(float);
			
			this.speed = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(float);
			
			this.state = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(int);
			
	    }
	
	    public bool Write(Span<byte> span, ref ushort count)
	    {
	        // 인자로 들어온 Span은 전체의 넓은 버퍼를 의미
	        bool serializeFlag = true;
	        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.GameObjectId);
			count += sizeof(int);
			
			ushort GameObjectNameLength = (ushort)Encoding.Unicode.GetBytes(this.GameObjectName.AsSpan(), span.Slice(count + 2, span.Length - (count + 2)));
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), GameObjectNameLength);
			count += sizeof(ushort);
			count += GameObjectNameLength;
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionX);
			count += sizeof(float);
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionY);
			count += sizeof(float);
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionZ);
			count += sizeof(float);
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.speed);
			count += sizeof(float);
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.state);
			count += sizeof(int);
			
	        return serializeFlag;
	    }
	
	    public int GetLength()
	    {
	        int entityLength = 0;
	
	        entityLength += sizeof(int);
			entityLength += 2 + (ushort)Encoding.Unicode.GetByteCount(this.GameObjectName.AsSpan());
			entityLength += sizeof(float);
			entityLength += sizeof(float);
			entityLength += sizeof(float);
			entityLength += sizeof(float);
			entityLength += sizeof(int);
	
	        return entityLength;
	    }
	}
	
	public List<GameObject> gameObjects = new List<GameObject>();
	
    public ushort Protocol { get { return (ushort)PacketID.STC_Spawn; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        gameObjects.Clear();
		ushort gameObjectLength = BitConverter.ToUInt16(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(ushort);
		
		for(int i = 0; i<gameObjectLength; i++)
		{
		    GameObject gameObject = new GameObject();
		    gameObject.Read(readSpan, ref count);
		    gameObjects.Add(gameObject);
		}
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;

        packetLength += 2;
        foreach (GameObject gameObject in this.gameObjects)
        {
	        packetLength += gameObject.GetLength();
        }

        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.STC_Spawn);
        count += sizeof(ushort);

        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)gameObjects.Count);
		count += sizeof(ushort);
		
		foreach (GameObject gameObject in this.gameObjects)
		    serializeFlag &= gameObject.Write(span, ref count);
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class STC_UnitSpawn : IPacket
{
    
	public struct GameObject
	{
	    public int GameObjectId;
		public string GameObjectName;
		public float positionX;
		public float positionY;
		public float positionZ;
		public int maxHp;
		public int hp;
		public float speed;
		public int state;
	
	    public void Read(ReadOnlySpan<byte> readSpan, ref ushort count)
	    {
	        this.GameObjectId = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(int);
			
			ushort GameObjectNameLength = BitConverter.ToUInt16(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(ushort);
			this.GameObjectName = Encoding.Unicode.GetString(readSpan.Slice(count, GameObjectNameLength));
			count += GameObjectNameLength;
			
			this.positionX = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(float);
			
			this.positionY = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(float);
			
			this.positionZ = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(float);
			
			this.maxHp = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(int);
			
			this.hp = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(int);
			
			this.speed = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(float);
			
			this.state = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(int);
			
	    }
	
	    public bool Write(Span<byte> span, ref ushort count)
	    {
	        // 인자로 들어온 Span은 전체의 넓은 버퍼를 의미
	        bool serializeFlag = true;
	        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.GameObjectId);
			count += sizeof(int);
			
			ushort GameObjectNameLength = (ushort)Encoding.Unicode.GetBytes(this.GameObjectName.AsSpan(), span.Slice(count + 2, span.Length - (count + 2)));
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), GameObjectNameLength);
			count += sizeof(ushort);
			count += GameObjectNameLength;
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionX);
			count += sizeof(float);
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionY);
			count += sizeof(float);
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionZ);
			count += sizeof(float);
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.maxHp);
			count += sizeof(int);
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.hp);
			count += sizeof(int);
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.speed);
			count += sizeof(float);
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.state);
			count += sizeof(int);
			
	        return serializeFlag;
	    }
	
	    public int GetLength()
	    {
	        int entityLength = 0;
	
	        entityLength += sizeof(int);
			entityLength += 2 + (ushort)Encoding.Unicode.GetByteCount(this.GameObjectName.AsSpan());
			entityLength += sizeof(float);
			entityLength += sizeof(float);
			entityLength += sizeof(float);
			entityLength += sizeof(int);
			entityLength += sizeof(int);
			entityLength += sizeof(float);
			entityLength += sizeof(int);
	
	        return entityLength;
	    }
	}
	
	public List<GameObject> gameObjects = new List<GameObject>();
	
    public ushort Protocol { get { return (ushort)PacketID.STC_UnitSpawn; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        gameObjects.Clear();
		ushort gameObjectLength = BitConverter.ToUInt16(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(ushort);
		
		for(int i = 0; i<gameObjectLength; i++)
		{
		    GameObject gameObject = new GameObject();
		    gameObject.Read(readSpan, ref count);
		    gameObjects.Add(gameObject);
		}
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;

        packetLength += 2;
        foreach (GameObject gameObject in this.gameObjects)
        {
	        packetLength += gameObject.GetLength();
        }

        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.STC_UnitSpawn);
        count += sizeof(ushort);

        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)gameObjects.Count);
		count += sizeof(ushort);
		
		foreach (GameObject gameObject in this.gameObjects)
		    serializeFlag &= gameObject.Write(span, ref count);
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class STC_Despawn : IPacket
{
    
	public struct GameObjectId
	{
	    public int ID;
	
	    public void Read(ReadOnlySpan<byte> readSpan, ref ushort count)
	    {
	        this.ID = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(int);
			
	    }
	
	    public bool Write(Span<byte> span, ref ushort count)
	    {
	        // 인자로 들어온 Span은 전체의 넓은 버퍼를 의미
	        bool serializeFlag = true;
	        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.ID);
			count += sizeof(int);
			
	        return serializeFlag;
	    }
	
	    public int GetLength()
	    {
	        int entityLength = 0;
	
	        entityLength += sizeof(int);
	
	        return entityLength;
	    }
	}
	
	public List<GameObjectId> gameObjectIds = new List<GameObjectId>();
	
    public ushort Protocol { get { return (ushort)PacketID.STC_Despawn; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        gameObjectIds.Clear();
		ushort gameObjectIdLength = BitConverter.ToUInt16(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(ushort);
		
		for(int i = 0; i<gameObjectIdLength; i++)
		{
		    GameObjectId gameObjectId = new GameObjectId();
		    gameObjectId.Read(readSpan, ref count);
		    gameObjectIds.Add(gameObjectId);
		}
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;

        packetLength += 2;
        foreach (GameObjectId gameObjectId in this.gameObjectIds)
        {
	        packetLength += gameObjectId.GetLength();
        }

        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.STC_Despawn);
        count += sizeof(ushort);

        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)gameObjectIds.Count);
		count += sizeof(ushort);
		
		foreach (GameObjectId gameObjectId in this.gameObjectIds)
		    serializeFlag &= gameObjectId.Write(span, ref count);
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class STC_DespawnProjectileAt : IPacket
{
    public int GameObjectId;
	public int positionX;
	public int positionY;
    public ushort Protocol { get { return (ushort)PacketID.STC_DespawnProjectileAt; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        this.GameObjectId = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.positionX = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.positionY = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.STC_DespawnProjectileAt);
        count += sizeof(ushort);

        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.GameObjectId);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionX);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionY);
		count += sizeof(int);
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class CTS_Move : IPacket
{
    public float positionX;
	public float positionY;
	public float positionZ;
	public float directionX;
	public float directionZ;
	public long timeStamp;
    public ushort Protocol { get { return (ushort)PacketID.CTS_Move; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        this.positionX = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(float);
		
		this.positionY = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(float);
		
		this.positionZ = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(float);
		
		this.directionX = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(float);
		
		this.directionZ = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(float);
		
		this.timeStamp = BitConverter.ToInt64(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(long);
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;
        packetLength += sizeof(float);
        packetLength += sizeof(float);
        packetLength += sizeof(float);
        packetLength += sizeof(float);
        packetLength += sizeof(float);
        packetLength += sizeof(long);
        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.CTS_Move);
        count += sizeof(ushort);

        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionX);
		count += sizeof(float);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionY);
		count += sizeof(float);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionZ);
		count += sizeof(float);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.directionX);
		count += sizeof(float);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.directionZ);
		count += sizeof(float);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.timeStamp);
		count += sizeof(long);
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class STC_Move : IPacket
{
    public int GameObjectId;
	public float positionX;
	public float positionY;
	public float positionZ;
	public float directionX;
	public float directionZ;
	public long timeStamp;
    public ushort Protocol { get { return (ushort)PacketID.STC_Move; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        this.GameObjectId = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.positionX = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(float);
		
		this.positionY = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(float);
		
		this.positionZ = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(float);
		
		this.directionX = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(float);
		
		this.directionZ = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(float);
		
		this.timeStamp = BitConverter.ToInt64(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(long);
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;
        packetLength += sizeof(int);
        packetLength += sizeof(float);
        packetLength += sizeof(float);
        packetLength += sizeof(float);
        packetLength += sizeof(float);
        packetLength += sizeof(float);
        packetLength += sizeof(long);
        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.STC_Move);
        count += sizeof(ushort);

        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.GameObjectId);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionX);
		count += sizeof(float);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionY);
		count += sizeof(float);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionZ);
		count += sizeof(float);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.directionX);
		count += sizeof(float);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.directionZ);
		count += sizeof(float);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.timeStamp);
		count += sizeof(long);
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class CTS_Idle : IPacket
{
    public float directionX;
	public float directionZ;
	public long timeStamp;
    public ushort Protocol { get { return (ushort)PacketID.CTS_Idle; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        this.directionX = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(float);
		
		this.directionZ = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(float);
		
		this.timeStamp = BitConverter.ToInt64(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(long);
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;
        packetLength += sizeof(float);
        packetLength += sizeof(float);
        packetLength += sizeof(long);
        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.CTS_Idle);
        count += sizeof(ushort);

        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.directionX);
		count += sizeof(float);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.directionZ);
		count += sizeof(float);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.timeStamp);
		count += sizeof(long);
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class STC_Idle : IPacket
{
    public int GameObjectId;
	public float directionX;
	public float directionZ;
	public long timeStamp;
    public ushort Protocol { get { return (ushort)PacketID.STC_Idle; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        this.GameObjectId = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.directionX = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(float);
		
		this.directionZ = BitConverter.ToSingle(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(float);
		
		this.timeStamp = BitConverter.ToInt64(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(long);
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;
        packetLength += sizeof(int);
        packetLength += sizeof(float);
        packetLength += sizeof(float);
        packetLength += sizeof(long);
        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.STC_Idle);
        count += sizeof(ushort);

        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.GameObjectId);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.directionX);
		count += sizeof(float);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.directionZ);
		count += sizeof(float);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.timeStamp);
		count += sizeof(long);
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class STC_Turn : IPacket
{
    public int GameObjectId;
	public int direction;
    public ushort Protocol { get { return (ushort)PacketID.STC_Turn; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        this.GameObjectId = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.direction = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.STC_Turn);
        count += sizeof(ushort);

        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.GameObjectId);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.direction);
		count += sizeof(int);
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class STC_FixPosition : IPacket
{
    public int GameObjectId;
	public int positionX;
	public int positionY;
	public int direction;
	public int state;
    public ushort Protocol { get { return (ushort)PacketID.STC_FixPosition; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        this.GameObjectId = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.positionX = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.positionY = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.direction = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.state = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.STC_FixPosition);
        count += sizeof(ushort);

        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.GameObjectId);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionX);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.positionY);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.direction);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.state);
		count += sizeof(int);
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class CTS_Skill : IPacket
{
    public int skillId;
    public ushort Protocol { get { return (ushort)PacketID.CTS_Skill; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        this.skillId = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;
        packetLength += sizeof(int);
        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.CTS_Skill);
        count += sizeof(ushort);

        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.skillId);
		count += sizeof(int);
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class STC_Skill : IPacket
{
    public int GameObjectId;
	public int skillId;
    public ushort Protocol { get { return (ushort)PacketID.STC_Skill; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        this.GameObjectId = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.skillId = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.STC_Skill);
        count += sizeof(ushort);

        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.GameObjectId);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.skillId);
		count += sizeof(int);
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class STC_ChangeHp : IPacket
{
    public int GameObjectId;
	public int hp;
    public ushort Protocol { get { return (ushort)PacketID.STC_ChangeHp; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        this.GameObjectId = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.hp = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.STC_ChangeHp);
        count += sizeof(ushort);

        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.GameObjectId);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.hp);
		count += sizeof(int);
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class STC_Die : IPacket
{
    public int GameObjectId;
	public int AttackerId;
    public ushort Protocol { get { return (ushort)PacketID.STC_Die; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        this.GameObjectId = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.AttackerId = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.STC_Die);
        count += sizeof(ushort);

        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.GameObjectId);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.AttackerId);
		count += sizeof(int);
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class CTS_Die : IPacket
{
    public int GameObjectId;
    public ushort Protocol { get { return (ushort)PacketID.CTS_Die; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        this.GameObjectId = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;
        packetLength += sizeof(int);
        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.CTS_Die);
        count += sizeof(ushort);

        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.GameObjectId);
		count += sizeof(int);
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class STC_Connected : IPacket
{
    
    public ushort Protocol { get { return (ushort)PacketID.STC_Connected; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;

        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.STC_Connected);
        count += sizeof(ushort);

        
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class CTS_RequestLogin : IPacket
{
    public string UniqueId;
    public ushort Protocol { get { return (ushort)PacketID.CTS_RequestLogin; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        ushort UniqueIdLength = BitConverter.ToUInt16(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(ushort);
		this.UniqueId = Encoding.Unicode.GetString(readSpan.Slice(count, UniqueIdLength));
		count += UniqueIdLength;
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;
        packetLength += 2 + (ushort)Encoding.Unicode.GetByteCount(this.UniqueId);
        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.CTS_RequestLogin);
        count += sizeof(ushort);

        ushort UniqueIdLength = (ushort)Encoding.Unicode.GetBytes(this.UniqueId, 0, this.UniqueId.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), UniqueIdLength);
		count += sizeof(ushort);
		count += UniqueIdLength;
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class STC_PermitLogin : IPacket
{
    public int PermitLogin;
	
	public struct Unit
	{
	    public string UnitName;
		public int level;
		public int characterType;
		public int strength;
		public int dexterity;
		public int resistance;
		public int proficiency;
		public int vitality;
	
	    public void Read(ReadOnlySpan<byte> readSpan, ref ushort count)
	    {
	        ushort UnitNameLength = BitConverter.ToUInt16(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(ushort);
			this.UnitName = Encoding.Unicode.GetString(readSpan.Slice(count, UnitNameLength));
			count += UnitNameLength;
			
			this.level = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(int);
			
			this.characterType = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(int);
			
			this.strength = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(int);
			
			this.dexterity = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(int);
			
			this.resistance = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(int);
			
			this.proficiency = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(int);
			
			this.vitality = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
			count += sizeof(int);
			
	    }
	
	    public bool Write(Span<byte> span, ref ushort count)
	    {
	        // 인자로 들어온 Span은 전체의 넓은 버퍼를 의미
	        bool serializeFlag = true;
	        ushort UnitNameLength = (ushort)Encoding.Unicode.GetBytes(this.UnitName.AsSpan(), span.Slice(count + 2, span.Length - (count + 2)));
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), UnitNameLength);
			count += sizeof(ushort);
			count += UnitNameLength;
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.level);
			count += sizeof(int);
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.characterType);
			count += sizeof(int);
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.strength);
			count += sizeof(int);
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.dexterity);
			count += sizeof(int);
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.resistance);
			count += sizeof(int);
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.proficiency);
			count += sizeof(int);
			
			serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.vitality);
			count += sizeof(int);
			
	        return serializeFlag;
	    }
	
	    public int GetLength()
	    {
	        int entityLength = 0;
	
	        entityLength += 2 + (ushort)Encoding.Unicode.GetByteCount(this.UnitName.AsSpan());
			entityLength += sizeof(int);
			entityLength += sizeof(int);
			entityLength += sizeof(int);
			entityLength += sizeof(int);
			entityLength += sizeof(int);
			entityLength += sizeof(int);
			entityLength += sizeof(int);
	
	        return entityLength;
	    }
	}
	
	public List<Unit> units = new List<Unit>();
	
    public ushort Protocol { get { return (ushort)PacketID.STC_PermitLogin; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        this.PermitLogin = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		units.Clear();
		ushort unitLength = BitConverter.ToUInt16(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(ushort);
		
		for(int i = 0; i<unitLength; i++)
		{
		    Unit unit = new Unit();
		    unit.Read(readSpan, ref count);
		    units.Add(unit);
		}
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;
        packetLength += sizeof(int);

        packetLength += 2;
        foreach (Unit unit in this.units)
        {
	        packetLength += unit.GetLength();
        }

        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.STC_PermitLogin);
        count += sizeof(ushort);

        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.PermitLogin);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)units.Count);
		count += sizeof(ushort);
		
		foreach (Unit unit in this.units)
		    serializeFlag &= unit.Write(span, ref count);
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class CTS_CreatNewUnit : IPacket
{
    public string UnitName;
    public ushort Protocol { get { return (ushort)PacketID.CTS_CreatNewUnit; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        ushort UnitNameLength = BitConverter.ToUInt16(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(ushort);
		this.UnitName = Encoding.Unicode.GetString(readSpan.Slice(count, UnitNameLength));
		count += UnitNameLength;
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;
        packetLength += 2 + (ushort)Encoding.Unicode.GetByteCount(this.UnitName);
        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.CTS_CreatNewUnit);
        count += sizeof(ushort);

        ushort UnitNameLength = (ushort)Encoding.Unicode.GetBytes(this.UnitName, 0, this.UnitName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), UnitNameLength);
		count += sizeof(ushort);
		count += UnitNameLength;
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
public class STC_ResponseCreatNewUnit : IPacket
{
    public string UnitName;
	public int level;
	public int characterType;
	public int strength;
	public int dexterity;
	public int resistance;
	public int proficiency;
	public int vitality;
    public ushort Protocol { get { return (ushort)PacketID.STC_ResponseCreatNewUnit; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> readSpan = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        ushort UnitNameLength = BitConverter.ToUInt16(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(ushort);
		this.UnitName = Encoding.Unicode.GetString(readSpan.Slice(count, UnitNameLength));
		count += UnitNameLength;
		
		this.level = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.characterType = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.strength = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.dexterity = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.resistance = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.proficiency = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
		this.vitality = BitConverter.ToInt32(readSpan.Slice(count, readSpan.Length - count));
		count += sizeof(int);
		
    }

    public ArraySegment<byte> Write()
    {
        // 4는 패킷 해더 길이값(ushort) + ID(ushort) 속도를 생각해서 4로 넣음
        int packetLength = 4;
        packetLength += 2 + (ushort)Encoding.Unicode.GetByteCount(this.UnitName);
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        packetLength += sizeof(int);
        ArraySegment<byte> segment = SendBufferPool.UsingBufferStart(packetLength);
        ushort count = 0;
        bool serializeFlag = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.STC_ResponseCreatNewUnit);
        count += sizeof(ushort);

        ushort UnitNameLength = (ushort)Encoding.Unicode.GetBytes(this.UnitName, 0, this.UnitName.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), UnitNameLength);
		count += sizeof(ushort);
		count += UnitNameLength;
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.level);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.characterType);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.strength);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.dexterity);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.resistance);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.proficiency);
		count += sizeof(int);
		
		serializeFlag &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.vitality);
		count += sizeof(int);
		
        serializeFlag &= BitConverter.TryWriteBytes(span, count);

        if (serializeFlag == false) return null;

        return segment;
    }
}
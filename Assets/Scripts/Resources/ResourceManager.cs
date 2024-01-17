using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    //private readonly Dictionary<string, GameObject> _tileDictionary = new Dictionary<string, GameObject>();

    public List<GameObject> Tiles;
    public GameObject Player;
    public GameObject Player2;

    public ResourceManager()
    {
        Tiles = new List<GameObject>()
        {
            Resources.Load<GameObject>("Prefabs/GrassLand/Tiles/Tile_Center"),
            Resources.Load<GameObject>("Prefabs/GrassLand/Tiles/Tile_Center1"),
            Resources.Load<GameObject>("Prefabs/GrassLand/Tiles/Tile_Center2"),
            Resources.Load<GameObject>("Prefabs/GrassLand/Tiles/Tile_Center3"),
            Resources.Load<GameObject>("Prefabs/GrassLand/Tiles/Tile_Center4"),
        };
        
        Player = Resources.Load<GameObject>("Prefabs/Unit/RPGHeroPBR");
        Player2 = Resources.Load<GameObject>("Prefabs/Unit/DoubleSword05");
    }

    public GameObject SpawnObject(GameObject obj)
    {
        return Object.Instantiate(obj);
    }

    public void DespawnObject(GameObject obj)
    {
        Object.Destroy(obj);
    }
}

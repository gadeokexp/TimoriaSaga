using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    //private readonly Dictionary<string, GameObject> _tileDictionary = new Dictionary<string, GameObject>();

    public GameObject Tile1;
    public GameObject Player;
    public GameObject Player2;

    public ResourceManager()
    {
        Tile1 = Resources.Load<GameObject>("Prefabs/GrassLand/Tiles/Tile_Center");
        Player = Resources.Load<GameObject>("Prefabs/Unit/RPGHeroPBR");
        Player2 = Resources.Load<GameObject>("Prefabs/Unit/DoubleSword05");
    }

    public GameObject SpawnObject(GameObject obj)
    {
        return Object.Instantiate(obj);
    }
}

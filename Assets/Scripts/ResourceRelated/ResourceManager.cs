using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    private readonly Dictionary<string, GameObject> _tileDictionary = new Dictionary<string, GameObject>();

    public GameObject Tile1;

    public ResourceManager()
    {
        Tile1 = Resources.Load<GameObject>("Prefabs/GrassLand/Tiles/Tile_Center");
    }

    public GameObject SpawnObject(GameObject obj)
    {
        return Object.Instantiate(obj);
    }
}

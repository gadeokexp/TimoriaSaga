using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    //private readonly Dictionary<string, GameObject> _tileDictionary = new Dictionary<string, GameObject>();

    public List<GameObject> Tiles;
    public GameObject Player;
    public GameObject Player2;
    public GameObject EffectHit;

    List<GameObject> PoolEffectHit;

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
        
        Player = Resources.Load<GameObject>("Prefabs/Unit/Player1");
        Player2 = Resources.Load<GameObject>("Prefabs/Unit/Player2");

        EffectHit = Resources.Load<GameObject>("Prefabs/Effects/EffectHit/EffectHit");
        PoolEffectHit = new List<GameObject>();
    }

    public GameObject SpawnObject(GameObject obj)
    {
        if (obj == EffectHit)
        {
            foreach (GameObject hitObject in PoolEffectHit)
            {
                if (hitObject.activeSelf == false)
                {
                    hitObject.SetActive(true);
                    return hitObject;
                }
            }

            GameObject newHitObject = Object.Instantiate(obj);
            PoolEffectHit.Add(newHitObject);
            return newHitObject;            
        }
        else
        {
            return Object.Instantiate(obj);
        }
    }

    public void DespawnObject(GameObject obj)
    {
        Object.Destroy(obj);
    }
}

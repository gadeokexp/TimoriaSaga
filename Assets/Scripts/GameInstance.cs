using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstance : MonoSingleton<GameInstance>
{
    ResourceManager resource;
    MapManager map;

    protected override void OnStart()
    {
        resource = ResourceManager.Instance;
        map = MapManager.Instance;
    }
}
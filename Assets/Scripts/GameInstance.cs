using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstance : MonoSingleton<GameInstance>
{
    ResourceManager resource;
    FieldManager map;
    ContentManager content;

    protected override void OnStart()
    {
        resource = ResourceManager.Instance;
        map = FieldManager.Instance;
        content = ContentManager.Instance;
    }
}
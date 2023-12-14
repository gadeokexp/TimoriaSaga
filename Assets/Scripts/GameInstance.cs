using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstance : MonoSingleton<GameInstance>
{
    ResourceManager resource;
    FieldManager map;
    ContentManager content;
    InputManager input;
    UnitManager unit;

    // 모노 싱글톤
    NetworkManager network;

    protected override void OnStart()
    {
        resource = ResourceManager.Instance;
        content = ContentManager.Instance;
        map = FieldManager.Instance;
        input = InputManager.Instance;
        unit = UnitManager.Instance;
        network = NetworkManager.Instance;
    }
}
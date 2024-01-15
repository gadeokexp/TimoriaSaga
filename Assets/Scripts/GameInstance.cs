using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInstance : MonoSingleton<GameInstance>
{
    // 일반 싱글톤
    ResourceManager _resource;
    FieldTileManager _map;
    ContentManager _content;
    InputManager _input;
    UnitManager _unit;
    UIManager _ui;
    DataManager _data;

    // 모노 싱글톤
    NetworkManager _network;

    protected override void OnStart()
    {
        _resource = ResourceManager.Instance;
        _content = ContentManager.Instance;
        _input = InputManager.Instance;
        _ui = UIManager.Instance;
        _map = FieldTileManager.Instance;
        _unit = UnitManager.Instance;
        _data = DataManager.Instance;
        _network = NetworkManager.Instance;
    }

    public void OperateGameNetwork()
    {
        NetworkManager.Instance.Init();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Field");
    }
}
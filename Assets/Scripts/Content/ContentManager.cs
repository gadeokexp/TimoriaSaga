using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System;

public class ContentManager : Singleton<ContentManager>
{
    TileDataHelper _tile;

    public TileDataHelper Tile => _tile;

    public ContentManager() 
    {
        _tile = Content<TileData, TileDataHelper>.Init() as TileDataHelper;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TileData : ContentData
{
    public Tile[] Tiles;
}

[Serializable]
public class Tile : ContentEntity
{
    public int ContentKey;
    public int PrefabIndex;     // 잔디 박힌 타일인지, 나무 박힌 타일인지, 프리팹으로 구분
    public int Type;            // 물 타입 같은 경우 수영하게 만들고자 한다. 경사타입은 충돌체가 다를것이다
    public int[] Position;
}

public class TileDataHelper : ContentDataHelper
{
    static string _contentFilePath = "Contents/Fields/Field_001";
    Dictionary<int, Tile> _tiles = new Dictionary<int, Tile>();


    // 컨테츠 핼퍼 기본 인터페이스

    public string GetFilePath()
    {
        return _contentFilePath;
    }

    public void Convert(ContentData dataBlcok)
    {
        TileData tileBlock = dataBlcok as TileData;

        foreach(Tile tile in tileBlock.Tiles)
        {
            if (!_tiles.ContainsKey(tile.ContentKey))
            {
                _tiles[tile.ContentKey] = tile;
            }
        }
    }

    public ContentEntity GetContentEntity(int index)
    {
        return _tiles[index];
    }

    public int GetContentLength()
    {
        return _tiles.Count;
    }


    // 타일 컨텐츠 인터페이스
    public Dictionary<int, Tile> GetTiles()
    {
        return _tiles;
    }
}

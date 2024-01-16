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
    public int PrefabIndex;     // �ܵ� ���� Ÿ������, ���� ���� Ÿ������, ���������� ����
    public int Type;            // �� Ÿ�� ���� ��� �����ϰ� ������� �Ѵ�. ���Ÿ���� �浹ü�� �ٸ����̴�
    public int[] Position;
}

public class TileDataHelper : ContentDataHelper
{
    static string _contentFilePath = "Contents/Fields/Field_001";
    Dictionary<int, Tile> _tiles = new Dictionary<int, Tile>();


    // ������ ���� �⺻ �������̽�

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


    // Ÿ�� ������ �������̽�
    public Dictionary<int, Tile> GetTiles()
    {
        return _tiles;
    }
}

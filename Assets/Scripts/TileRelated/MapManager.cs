using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    Dictionary<Vector3, GameObject> tileDictionary;

    public MapManager()
    {
        tileDictionary = new Dictionary<Vector3, GameObject>();

        GameObject b = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.Tile1);
        b.transform.position = new Vector3(0, 0, 0);
        tileDictionary.Add(b.transform.position, b);

        GameObject c = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.Tile1);
        c.transform.position = new Vector3(0, 0, 2);
        tileDictionary.Add(c.transform.position, c);

        GameObject a = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.Tile1);
        a.transform.position = new Vector3(2, 0, 2);
        tileDictionary.Add(a.transform.position, a);

        GameObject d = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.Tile1);
        d.transform.position = new Vector3(2, 0, 0);
        tileDictionary.Add(d.transform.position, d);

        GameObject e = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.Tile1);
        e.transform.position = new Vector3(0, 2, 0);
        tileDictionary.Add(e.transform.position, e);

        GameObject f = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.Tile1);
        f.transform.position = new Vector3(2, 2, 0);
        tileDictionary.Add(f.transform.position, f);

        GameObject g = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.Tile1);
        g.transform.position = new Vector3(2, 2, 2);
        tileDictionary.Add(g.transform.position, g);

        GameObject h = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.Tile1);
        h.transform.position = new Vector3(0, 2, 2);
        tileDictionary.Add(h.transform.position, h);

        GameObject i = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.Tile1);
        i.transform.position = new Vector3(0, -2, 0);
        tileDictionary.Add(i.transform.position, i);

        GameObject j = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.Tile1);
        j.transform.position = new Vector3(2, -2, 0);
        tileDictionary.Add(j.transform.position, j);

        GameObject k = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.Tile1);
        k.transform.position = new Vector3(2, -2, 2);
        tileDictionary.Add(k.transform.position, k);

        //GameObject l = ResourceManager.Instance.SpawnObject(ResourceManager.Instance.Tile1);
        //l.transform.position = new Vector3(0, -2, 2);
        //tileDictionary.Add(l.transform.position, l);

        InitCollider();
    }

    class TileBlockSquare
    {
        // 화면에서 가까운 쪽
        public Vector3 nearLeftDown;
        public Vector3 nearRightDown;
        public Vector3 nearLeftUp;
        public Vector3 nearRightUp;

        // 화면에서 먼 쪽
        public Vector3 farLeftDown;
        public Vector3 farRightDown;
        public Vector3 farLeftUp;
        public Vector3 farRightUp;

        public TileBlockSquare(Vector3 firstSquare)
        {
            nearLeftDown = firstSquare;
            nearRightDown = firstSquare;
            nearLeftUp = firstSquare;
            nearRightUp = firstSquare;
            farLeftDown = firstSquare;
            farRightDown = firstSquare;
            farLeftUp = firstSquare;
            farRightUp = firstSquare;
        }
    }

    void InitCollider()
    {
        // 타일의 충돌체 생성 최적화 루틴
        HashSet<Vector3> openTiles = new HashSet<Vector3>(tileDictionary.Keys.ToHashSet());
        HashSet<Vector3> closedTiles = new HashSet<Vector3>();

        while (openTiles.Count > 0)
        {
            var firstTile = openTiles.ElementAt(0);
            openTiles.Remove(firstTile);
            closedTiles.Add(firstTile);

            // 첫번째 블럭의 충돌체 박스 좌표
            TileBlockSquare square = new TileBlockSquare(firstTile);

            bool[] expendable = new bool[6] { true, true, true, true, true, true }; // 상, 하, 좌, 우, 니어, 파

            List<Vector3> tileBlock = new List<Vector3>();

            while (expendable[0] || expendable[1] || expendable[2] || expendable[3] || expendable[4] || expendable[5])
            {
                if (expendable[0])
                {
                    for (int x = (int)square.nearLeftUp.x; x <= (int)square.nearRightUp.x; x = x + 2)
                    {
                        for (int z = (int)square.nearLeftUp.z; z <= (int)square.farLeftUp.z; z = z + 2)
                        {
                            Vector3 eachTilePosition = new Vector3(x, square.nearLeftUp.y + 2f, z);

                            if (openTiles.Contains(eachTilePosition))
                            {
                                tileBlock.Add(eachTilePosition);
                            }
                            else
                            {
                                tileBlock.Clear();
                                z = (int)square.farLeftUp.z + 1;
                                x = (int)square.nearRightUp.x + 1;
                                expendable[0] = false;
                            }
                        }
                    }

                    if (tileBlock.Count > 0)
                    {
                        foreach (var blockTile in tileBlock)
                        {
                            openTiles.Remove(blockTile);
                            closedTiles.Add(blockTile);
                        }

                        square.nearLeftUp += Vector3.up * 2;
                        square.nearRightUp += Vector3.up * 2;
                        square.farLeftUp += Vector3.up * 2;
                        square.farRightUp += Vector3.up * 2;
                        tileBlock.Clear();
                    }
                }

                if (expendable[1])
                {
                    for (int x = (int)square.nearLeftDown.x; x <= (int)square.nearRightDown.x; x = x + 2)
                    {
                        for (int z = (int)square.nearLeftDown.z; z <= (int)square.farLeftDown.z; z = z + 2)
                        {
                            Vector3 eachTilePosition = new Vector3(x, square.nearLeftDown.y - 2f, z);

                            if (openTiles.Contains(eachTilePosition))
                            {
                                tileBlock.Add(eachTilePosition);
                            }
                            else
                            {
                                tileBlock.Clear();
                                z = (int)square.farLeftDown.z + 1;
                                x = (int)square.nearRightDown.x + 1;
                                expendable[1] = false;
                            }
                        }
                    }

                    if (tileBlock.Count > 0)
                    {
                        foreach (var blockTile in tileBlock)
                        {
                            openTiles.Remove(blockTile);
                            closedTiles.Add(blockTile);
                        }

                        square.nearLeftDown += Vector3.down * 2;
                        square.nearRightDown += Vector3.down * 2;
                        square.farLeftDown += Vector3.down * 2;
                        square.farRightDown += Vector3.down * 2;
                        tileBlock.Clear();
                    }
                }

                if (expendable[2])
                {
                    for (int y = (int)square.nearLeftDown.y; y <= (int)square.nearLeftUp.y; y = y + 2)
                    {
                        for (int z = (int)square.nearLeftDown.z; z <= (int)square.farLeftDown.z; z = z + 2)
                        {
                            Vector3 eachTilePosition = new Vector3(square.nearLeftDown.x - 2f, y, z);

                            if (openTiles.Contains(eachTilePosition))
                            {
                                tileBlock.Add(eachTilePosition);
                            }
                            else
                            {
                                tileBlock.Clear();
                                z = (int)square.farLeftDown.z + 1;
                                y = (int)square.nearLeftUp.y + 1;
                                expendable[2] = false;
                            }
                        }
                    }

                    if (tileBlock.Count > 0)
                    {
                        foreach (var blockTile in tileBlock)
                        {
                            openTiles.Remove(blockTile);
                            closedTiles.Add(blockTile);
                        }

                        square.nearLeftUp += Vector3.left * 2;
                        square.nearLeftDown += Vector3.left * 2;
                        square.farLeftUp += Vector3.left * 2;
                        square.farLeftDown += Vector3.left * 2;
                        tileBlock.Clear();
                    }
                }

                if (expendable[3])
                {
                    for (int y = (int)square.nearRightDown.y; y <= (int)square.nearRightUp.y; y = y + 2)
                    {
                        for (int z = (int)square.nearRightDown.z; z <= (int)square.farRightDown.z; z = z + 2)
                        {
                            Vector3 eachTilePosition = new Vector3(square.nearRightDown.x + 2f, y, z);

                            if (openTiles.Contains(eachTilePosition))
                            {
                                tileBlock.Add(eachTilePosition);
                            }
                            else
                            {
                                tileBlock.Clear();
                                z = (int)square.farRightDown.z + 1;
                                y = (int)square.nearRightUp.y + 1;
                                expendable[3] = false;
                            }
                        }
                    }

                    if (tileBlock.Count > 0)
                    {
                        foreach (var blockTile in tileBlock)
                        {
                            openTiles.Remove(blockTile);
                            closedTiles.Add(blockTile);
                        }

                        square.nearRightUp += Vector3.right * 2;
                        square.nearRightDown += Vector3.right * 2;
                        square.farRightUp += Vector3.right * 2;
                        square.farRightDown += Vector3.right * 2;
                        tileBlock.Clear();
                    }
                }

                if (expendable[4])
                {
                    for (int x = (int)square.nearLeftDown.x; x <= (int)square.nearRightDown.x; x = x + 2)
                    {
                        for (int y = (int)square.nearLeftDown.y; y <= (int)square.nearLeftUp.y; y = y + 2)
                        {
                            Vector3 eachTilePosition = new Vector3(x, y, square.nearLeftDown.z - 2f);

                            if (openTiles.Contains(eachTilePosition))
                            {
                                tileBlock.Add(eachTilePosition);
                            }
                            else
                            {
                                tileBlock.Clear();
                                y = (int)square.nearLeftUp.y + 1;
                                x = (int)square.nearRightDown.x + 1;
                                expendable[4] = false;
                            }
                        }
                    }

                    if (tileBlock.Count > 0)
                    {
                        foreach (var blockTile in tileBlock)
                        {
                            openTiles.Remove(blockTile);
                            closedTiles.Add(blockTile);
                        }

                        square.nearRightUp += Vector3.back * 2;
                        square.nearRightDown += Vector3.back * 2;
                        square.nearLeftUp += Vector3.back * 2;
                        square.nearLeftDown += Vector3.back * 2;
                        tileBlock.Clear();
                    }
                }

                if (expendable[5])
                {
                    for (int x = (int)square.farLeftDown.x; x <= (int)square.farRightDown.x; x = x + 2)
                    {
                        for (int y = (int)square.farLeftDown.y; y <= (int)square.farLeftUp.y; y = y + 2)
                        {
                            Vector3 eachTilePosition = new Vector3(x, y, square.farLeftDown.z + 2f);

                            if (openTiles.Contains(eachTilePosition))
                            {
                                tileBlock.Add(eachTilePosition);
                            }
                            else
                            {
                                tileBlock.Clear();
                                y = (int)square.farLeftUp.y + 1;
                                x = (int)square.farRightDown.x + 1;
                                expendable[5] = false;
                            }
                        }
                    }

                    if (tileBlock.Count > 0)
                    {
                        foreach (var blockTile in tileBlock)
                        {
                            openTiles.Remove(blockTile);
                            closedTiles.Add(blockTile);
                        }

                        square.farRightUp += Vector3.forward * 2;
                        square.farRightDown += Vector3.forward * 2;
                        square.farLeftUp += Vector3.forward * 2;
                        square.farLeftDown += Vector3.forward * 2;
                        tileBlock.Clear();
                    }
                }
            }
            var coll = tileDictionary[firstTile].AddComponent<BoxCollider>();

            coll.size = new Vector3(Mathf.Abs(square.nearRightDown.x - square.nearLeftDown.x) + 2,
                Mathf.Abs(square.nearLeftUp.y - square.nearLeftDown.y) + 2,
                Mathf.Abs(square.farLeftDown.z - square.nearLeftDown.z) + 2);

            coll.center = (square.nearLeftDown + square.farRightUp) / 2 - firstTile;
        }
    }
}

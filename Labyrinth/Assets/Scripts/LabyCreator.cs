using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LabyCreator
{
    private static Tile[,] tiles;
    private static int[,] tileTypes;
    private static int size;
    private static Vector2Int startTile, endTile;
    private static float placementThreshold;
    private static Player player;
    private static Sprite playerSprite;
    private static Main main;
    private static bool hide = true;

    public static Sprite PlayerSprite { get => playerSprite; set => playerSprite = value; }
    public static Main Main { set => main = value; }

    public static int TryMove(Vector2Int move)
    {
        Vector2Int tstTile = move + player.Coords;
        if (tstTile.x >= 0 && tstTile.y >= 0 && tstTile.x < size && tstTile.y < size)
        {
            player.Move(move);
            if (tileTypes[tstTile.x, tstTile.y] == 0)
            {
                player.Destroy();
                if (hide)
                    tiles[tstTile.x, tstTile.y].Enabled(false);
            }
            return tileTypes[tstTile.x, tstTile.y];
        }
        return 1;
    }

    public static void resetPlayer()
    {
        player = new Player(startTile, 25f / size, PlayerSprite);
    }

    public static void Colour()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                tiles[x, y].Colour();
            }
        }
    }
    public static void ResetColour()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                tiles[x, y].ResetColour();
            }
        }
    }
    public static bool FlipHideOnKill()
    {
        hide = !hide;
        if (!hide)
        {
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    tiles[x, y].Enabled(true);
                }
            }
        }
        return hide;
    }

    public static void Disable()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                tiles[x, y].Destroy();
            }
        }
        player.Destroy();
    }

    public static void DestroyPlayer()
    {
        player.Destroy();
    }
    public static void DestroyGameObject(GameObject toDestroy)
    {
        main.DestroyGameObject(toDestroy);
    }


    private static void GenerateNewMaze()
    {
        tileTypes = new int[size, size];
        
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                tileTypes[i, j] = 1;
                if (i % 2 == 0 && j % 2 == 0)
                {
                    if (Random.value > placementThreshold)
                    {
                        tileTypes[i, j] = 0;

                        int a = Random.value < .5 ? 0 : (Random.value < .5 ? -1 : 1);
                        int b = a != 0 ? 0 : (Random.value < .5 ? -1 : 1);
                        if (InBounds(i + a, j + b))
                            tileTypes[i + a, j + b] = 0;
                    }
                }
            }
        }
    }

    private static bool InBounds(int x, int y)
    {
        if (x >= 0 && x < size && y >= 0 && y < size)
            return true;
        return false;
    }

    private static void AddStartEnd()
    {
        int[,] tileZones = new int[size, size];
        List<Vector2Int> queue = new List<Vector2Int>();
        List<Vector2Int> groupQueue = new List<Vector2Int>();
        Vector2Int[] mods = new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        Vector2Int tile = new Vector2Int();
        Vector2Int conn;
        int group = 0;
        queue.Add(new Vector2Int(0, 0));

        while (queue.Count != 0)
        {
            tile = queue[0];
            queue.RemoveAt(0);
            if (tileZones[tile.x, tile.y] == 0)//not already grouped
            {
                if (tileTypes[tile.x, tile.y] == 1)//if passable
                {
                    groupQueue = new List<Vector2Int>();
                    groupQueue.Add(tile);
                    BFSGroup();
                }
                else
                {
                    tileZones[tile.x, tile.y] = -1;
                    for (int i = 0; i < 4; i++)
                        TryAdd(tile + mods[i]);
                }
            }
        }

        void TryAdd(Vector2Int tileIn)
        {
            if (InBounds(tileIn.x, tileIn.y))
                queue.Add(tileIn);
        }

        void BFSGroup()
        {
            group++;
            while (groupQueue.Count != 0)
            {
                tile = groupQueue[0];
                groupQueue.RemoveAt(0);
                if (tileZones[tile.x, tile.y] == 0)//not already grouped
                {
                    if (tileTypes[tile.x, tile.y] == 1)//if passable
                    {
                        tileZones[tile.x, tile.y] = group;
                        for (int i = 0; i < 4; i++)
                        {
                            conn = tile + mods[i];
                            if (InBounds(conn.x, conn.y))
                            {
                                if (tileTypes[conn.x, conn.y] == 1)//if passable
                                    groupQueue.Add(conn);
                                else
                                    TryAdd(conn);
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < size; i++)
        {
            string[] tmp = new string[size];
            for (int j = 0; j < size; j++)
            {
                tmp[j] = tileZones[i, j].ToString();
            }
        }
        Dictionary<int, int> groupCounts = new Dictionary<int, int>();
        int zone;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                zone = tileZones[i, j];
                if (zone > 0)
                {
                    if (!groupCounts.ContainsKey(zone))
                        groupCounts.Add(zone, 0);
                    groupCounts[zone] += 1;
                }
            }
        }
        int bestZone = 0;
        int bestZoneCount = 0;
        foreach (int key in groupCounts.Keys)
        {
            if (groupCounts[key] > bestZoneCount)
            {
                bestZone = key;
                bestZoneCount = groupCounts[key];
            }
        }
        Vector2Int selectedTile = new Vector2Int(Random.Range(0, size), Random.Range(0, size));
        bool found = false;
        while (!found)
        {
            selectedTile = new Vector2Int(Random.Range(0, size), Random.Range(0, size));
            if (tileZones[selectedTile.x, selectedTile.y] == bestZone)
                found = true;
        }
        startTile = selectedTile;
        tileTypes[startTile.x, startTile.y] = 3;

        queue.Add(startTile);
        List<Vector2Int> completed = new List<Vector2Int>();
        while (queue.Count != 0)
        {
            if (!completed.Contains(queue[0]))
            {
                tile = queue[0];
                for (int i = 0; i < 4; i++)
                {
                    conn = tile + mods[i];
                    if (InBounds(conn.x, conn.y))
                        if (tileZones[conn.x, conn.y] == tileZones[startTile.x, startTile.y])
                            queue.Add(conn);
                }
                completed.Add(tile);
            }
            queue.RemoveAt(0);
        }
        endTile = tile;
        tileTypes[endTile.x, endTile.y] = 2;
    }



    

    public static void CreateLabyrinth(int sizeIn, Sprite[] tileSprites)
    {
        size = sizeIn;
        tiles = new Tile[size, size];
        placementThreshold = 0.01f;
        GenerateNewMaze();
        AddStartEnd();

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                tiles[x, y] = new Tile(new Vector2Int(x, y), tileTypes[x, y], 25f / size, tileSprites[Random.Range(0, tileSprites.Length)]);
                if (tileTypes[x, y] == 3)
                    startTile = new Vector2Int(x, y);
                if (tileTypes[x, y] == 2)
                    endTile = new Vector2Int(x, y);
            }
        }
        tiles[startTile.x, startTile.y].Colour();
        tiles[endTile.x, endTile.y].Colour();
        resetPlayer();
    }

}

﻿using System.IO;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MapGenerator : MonoBehaviour {
    public TextAsset map;
    public GameObject[] tilePrefabs;
    public Transform parent;
    public static readonly int step = 3;
    private Tile[,] tiles;
    public GameObject[,] tileObjects;


	void Awake () {
        StringReader sr = new StringReader(map.text);
        string line;
        ArrayList rows = new ArrayList();
        int width = 0;
        while((line = sr.ReadLine()) != null)
        {
            ArrayList row = new ArrayList();
            foreach(char c in line)
            {
                if (c == ',')
                    continue;
                int tileInt = (int)char.GetNumericValue(c);
                row.Add(tileInt);
            }
            width = row.Count;
            rows.Add(row);
        }
        int height = rows.Count;
        tiles = new Tile[width, height];
        int x = 0;
        int y = 0;
        tileObjects = new GameObject[width, height];
        foreach (ArrayList arr in rows)
        {
            foreach(int k in arr)
            {
                tiles[x, height - 1 - y] = Tile.GetTile(k);
                GameObject tile = Instantiate(tilePrefabs[k], new Vector3(x * step, 0, (height - 1 - y) * step), Quaternion.identity, parent);

                // Set up NavMesh
                NavMeshSurface surface = tile.GetComponent<NavMeshSurface>();
                if (surface != null)
                {
                    surface.BuildNavMesh();
                }

                // Set up individual tile info
                TileInfo info = tile.GetComponent<TileInfo>();
                if (info == null) {
                    info = tile.AddComponent<TileInfo>();
                }
                info.x = x;
                info.y = height - y - 1;

                tileObjects[x, height - y - 1] = tile;

                x++;
            }
            y++;
            x = 0;
        }

	}

    public Tile GetTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= tiles.GetLength(0) || y >= tiles.GetLength(1))
            return null;
        return tiles[x, y];
    }
	public Tile.TileType GetTileType(int x, int y)
	{
		if (x < 0 || y < 0 || x >= tiles.GetLength (0) || y >= tiles.GetLength (1))
			return Tile.TileType.unknown;
		return tiles[x, y].Type;
	}

    public int SizeX
    {
        get { return tiles.GetLength(0); }
    }

    public int SizeY
    {
        get { return tiles.GetLength(1); }
    }
}

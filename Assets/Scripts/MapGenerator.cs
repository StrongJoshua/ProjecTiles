using System.IO;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public TextAsset map;
    public GameObject[] tilePrefabs;
    public int step;
    private int[,] tiles;
	// Use this for initialization
	void Start () {
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
        tiles = new int[width, height];
        int x = 0;
        int y = 0;
        foreach(ArrayList arr in rows)
        {
            foreach(int k in arr)
            {
                tiles[x, height - 1 - y] = k;
                Instantiate(tilePrefabs[k], new Vector3(x * step, 0, (height - 1 - y) * step), Quaternion.identity);
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
        return Tile.GetTile(tiles[x, y]);
    }
	public Tile.TileType GetTileType(int x, int y)
	{
		if (x < 0 || y < 0 || x >= tiles.GetLength (0) || y >= tiles.GetLength (1))
			return Tile.TileType.unknown;
		return (Tile.TileType)tiles [x, y];
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

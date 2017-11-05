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

        tiles = new int[rows.Count, width];
        int i = 0;
        int j = 0;
        foreach(ArrayList arr in rows)
        {
            foreach(int k in arr)
            {
                tiles[i, j] = k;
                Instantiate(tilePrefabs[k], new Vector3((tiles.GetLength(0) - j - 1) * step, 0, i * step), Quaternion.identity);
                j++;
                Debug.Log(k);
            }
            i++;
            j = 0;
        }
	}

    public Tile GetTile(int x, int y)
    {
        return Tile.GetTile(tiles[x, y]);
    }
}

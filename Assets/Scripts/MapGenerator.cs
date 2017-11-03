using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {
    public TextAsset map;
    public GameObject[] tilePrefabs;
    private Dictionary<Point, int> tiles;
	// Use this for initialization
	void Start () {
        tiles = new Dictionary<Point, int>();
        StringReader sr = new StringReader(map.text);
        string line;
        int x = 0, z = 0;
        while((line = sr.ReadLine()) != null)
        {
            foreach(char c in line)
            {
                if (c == ',')
                    continue;
                int tileInt = (int)char.GetNumericValue(c);
                Instantiate(tilePrefabs[tileInt], new Vector3(x, 0, z), Quaternion.identity);
                tiles.Add(new Point(x, z), tileInt);
            }
            x = 0;
            z++;
        }
	}

    public int getTile(Point p)
    {
        int val;
        if (tiles.TryGetValue(p, out val)) return val;
        throw new Exception("Tile " + p + " does not exist.");
    }
}

using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System;

public class MapGenerator : MonoBehaviour
{
	public TextAsset map;
	public GameObject[] plainTilePrefabs;
	public GameObject[] waterTilePrefabs;
	public GameObject[] hillTilePrefabs;
	public GameObject[] swampTilePrefabs;
	public GameObject[] forestTilePrefabs;
	public GameObject[] sandTilePrefabs;
	public GameObject[] boulderTilePrefabs;
	public GameObject[] explosiveTilePrefabs;
	public GameObject[] rampTilePrefabs;
	

	public GameObject unknownTilePrefab;

	public Transform parent;
	public static readonly int step = 3;
	private Tile[,] tiles;
	public GameObject[,] tileObjects;
	public GameObject[,] highlights;
	public GameObject highlightPlane;
	public Color highlightColor;
	public float rateOfDesctructibleTiles;
	public GameObject water;
	public Camera minimap;
	public List<GameObject> rampTiles;

	private void Awake ()
	{
		if (FindObjectOfType<GameManager>() == null)
			generateMap (null);

		rampTiles = new List<GameObject> ();
	}

	public List<List<int>> readText ()
	{
		StringReader sr = new StringReader (map.text);
		string line;
		List<List<int>> rows = new List<List<int>> ();
		int width = 0;
		while ((line = sr.ReadLine ()) != null) {
			List<int> row = new List<int>();
			foreach (char c in line) {
				if (c == ',')
					continue;
				int tileInt = (int)char.GetNumericValue (c);
				row.Add (tileInt);
			}
			width = row.Count;
			rows.Add (row);
		}
		return rows;
	}

	internal void generateMap (Action tileChangedCallback)
	{
        List<List<int>> rows = readText();
        int width = rows[0].Count;

		int height = rows.Count;
		tiles = new Tile[width, height];
		int x = 0;
		int y = 0;

		tileObjects = new GameObject[width, height];
		highlights = new GameObject[width, height];

		foreach (List<int> arr in rows) {
			foreach (int k in arr) {
				tiles [x, height - 1 - y] = Tile.GetTile (k);

				Tile.TileType tileType = (Tile.TileType)k;
				GameObject[] choices;
				if (tileType == Tile.TileType.plain)
					choices = plainTilePrefabs;
				else if (tileType == Tile.TileType.water)
					choices = waterTilePrefabs;
				else if (tileType == Tile.TileType.hill)
					choices = hillTilePrefabs;
				else if (tileType == Tile.TileType.swamp)
					choices = swampTilePrefabs;
				else if (tileType == Tile.TileType.forest)
					choices = forestTilePrefabs;
				else if (tileType == Tile.TileType.sand)
					choices = sandTilePrefabs;
				else if (tileType == Tile.TileType.boulder)
					choices = boulderTilePrefabs;
				else if (tileType == Tile.TileType.explosive)
					choices = explosiveTilePrefabs;
				else if (tileType == Tile.TileType.ramp)
					choices = rampTilePrefabs;
				else
					choices = null;


				GameObject tile = Instantiate (choices == null ? unknownTilePrefab : choices [UnityEngine.Random.Range (0, choices.Length)], new Vector3 (x * step, 0, (height - 1 - y) * step),
					                              Quaternion.identity, parent);
                tile.tag = "ground";

                TileManager tm = tile.GetComponent<TileManager>();
                if (tm != null)
                    tm.tileChangedCallback = tileChangedCallback;

				GameObject highlight = Instantiate (highlightPlane, tile.transform.position + new Vector3 (0, .6f, 0), Quaternion.identity, tile.transform);
				highlight.SetActive (false);
				highlight.GetComponent<MeshRenderer> ().material.color = highlightColor;

				// Set up individual tile info
				TileInfo info = tile.GetComponent<TileInfo> ();
				if (info == null) {
					info = tile.AddComponent<TileInfo> ();
				}
				info.x = x;
				info.y = height - y - 1;

				if (choices == rampTilePrefabs) {
					rampTiles.Add (tile);
				}

				tileObjects [x, height - y - 1] = tile;
				highlights [x, height - y - 1] = highlight;

				x++;
			}
			y++;
			x = 0;
		}
		GameObject waterInt = Instantiate (water);
		waterInt.transform.localScale = new Vector3 (SizeX * step / 10f, 0, SizeY * step / 10f);
		waterInt.transform.position = new Vector3 (SizeX * step / 2f - step / 2f, 0.4f, SizeY * step / 2f - step / 2f);
		if (minimap != null) {
			minimap.orthographicSize = SizeX * step / 2f;
			minimap.transform.position = new Vector3 (SizeX * step / 2f, 120, SizeY * step / 2.1f);
		}
	}

	public Tile GetTile (int x, int y)
	{
		if (x < 0 || y < 0 || x >= tiles.GetLength (0) || y >= tiles.GetLength (1))
			return null;
		return tiles [x, y];
	}

	public Tile.TileType GetTileType (int x, int y)
	{
		if (x < 0 || y < 0 || x >= tiles.GetLength (0) || y >= tiles.GetLength (1))
			return Tile.TileType.unknown;
		return tiles [x, y].Type;
	}

	public int SizeX {
		get { return tiles.GetLength (0); }
	}

	public int SizeY {
		get { return tiles.GetLength (1); }
	}

	public Tile[,] Tiles {
		get { return tiles; }
	}

	public bool hasGeneratedMap ()
	{
		return tiles != null;
	}

	public List<Tile> GetAdjacentTiles(int x, int y) {
		List<Tile> adjacentTiles = new List<Tile> ();

		Tile down = GetTile (x, y - 1);
		//if (down != null)
		adjacentTiles.Add (down);

		Tile left = GetTile (x - 1, y);
		//if (left != null)
		adjacentTiles.Add (left);

		Tile up = GetTile (x, y + 1);
		//if (up != null)
		adjacentTiles.Add (up);

		Tile right = GetTile (x + 1, y);
		//if (right != null)
		adjacentTiles.Add (right);

	

		return adjacentTiles;
	}

	public void rotateRamps() {
		foreach (GameObject ramp in rampTiles) {
			if (ramp != null) {
				TileInfo info = ramp.GetComponent<TileInfo> ();
				List<Tile> adjTiles = GetAdjacentTiles (info.x, info.y);

				foreach (Tile adjTile in adjTiles) {
					if (adjTile != null && adjTile.Type == Tile.TileType.hill) {
						ramp.transform.Rotate (new Vector3 (0, 90 * adjTiles.IndexOf (adjTile), 0));
					}
				}
			}
		}
	}

}

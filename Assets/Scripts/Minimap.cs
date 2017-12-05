using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour {
	public Texture2D tileset;
	public Texture2D minimap;
	public GameManager game;
	public MapGenerator map;
	private bool created = false;
	// Use this for initialization
	void Start () {
		minimap = GetComponent<RawImage> ().texture as Texture2D;
	}
	
	// Update is called once per frame
	void Update () {
		if (!created) {
			int x = 0;
			int y = 0;
			ArrayList tiles = map.readText ();
			foreach (ArrayList arr in tiles) {
				foreach (int k in arr) {
					minimap.SetPixel (x, y, new Color (tileset.GetPixel (k, 1).r, tileset.GetPixel (k, 1).g, tileset.GetPixel (k, 1).b));
					//print (tileset.GetPixel (k, 1).ToString() + "");
					x++;
				}
				y++;
			}
			created = true;
		}
		minimap.SetPixel (5,5,Color.red);
		
	}
}

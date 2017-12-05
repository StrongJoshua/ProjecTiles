﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour {
	public Texture2D tileset;
	public Texture2D minimap;
	public GameManager game;
	public MapGenerator map;
	private bool created;
	// Use this for initialization
	void Start () {
		created = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!created) {
			int x = 0;
			int y = 0;
			ArrayList tiles = map.readText ();
			foreach (ArrayList arr in tiles) {
				foreach (int k in arr) {
					minimap.SetPixel (x, y, tileset.GetPixel(k,1));
					print (minimap.GetPixel (x,y).ToString() + "");
					x++;
				}
				y++;
			}
			created = true;
			gameObject.AddComponent<RawImage> ();
			GetComponent<RawImage> ().texture = minimap;

		}
	}
}

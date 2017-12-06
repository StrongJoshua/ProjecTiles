using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour {
	public Texture2D tileset;
	private Texture2D minimap;
	public GameManager game;
	public MapGenerator map;
    public int scale;
	private bool created;
	// Use this for initialization
	void Start () {
		created = false;
	}
	
	// Update is called once per frame
	void Update () {
<<<<<<< HEAD
        if (created)
            return;

        updateForCharacters();

        created = true;
=======
		if (!created) {
			int x = 0;
			int y = 0;
			ArrayList tiles = map.readText ();
			foreach (ArrayList arr in tiles) {
				foreach (int k in arr) {
					minimap.SetPixel (x, y, tileset.GetPixel(k,1));
					//print (minimap.GetPixel (x,y).ToString() + "");
					x++;
				}
				y++;
				x = 0;
			}
			Color[] colors  = new Color[minimap.height*minimap.width];
			for (int i = 0; i < colors.Length; i++) {
				colors [i] = new Color (0.5f, 0.5f, 0.5f,1f);
			}
			minimap.SetPixels (colors);
			//print(x + " " + y + " " + minimap.height);
			//created = true;
			//gameObject.AddComponent<RawImage> ();
			GetComponent<RawImage> ().texture = minimap as Texture;

		}
>>>>>>> 9551e160819195d07cd7fb29ff5c15b91338eb47
	}

    private void update()
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(map.SizeX * scale, map.SizeY * scale);

        minimap = new Texture2D(map.SizeX, map.SizeY, TextureFormat.ARGB4444, false);
        minimap.filterMode = FilterMode.Point;

        int x = 0;
        int y = 0;
        List<List<int>> tiles = map.readText();
        foreach (List<int> arr in tiles)
        {
            foreach (int k in arr)
            {
                Color c = tileset.GetPixel(k % tileset.width, tileset.height - 1 - k / tileset.width);
                print(c);
                minimap.SetPixel(x, map.SizeY - 1 - y, c);
                x++;
            }
            y++;
            x = 0;
        }

        minimap.Apply();
        GetComponent<RawImage>().texture = minimap;
    }

    public void updateForCharacters()
    {
        update();

        foreach(Unit u in game.player.units)
        {
            if (u.IsDead)
                continue;
            drawCircle(u.XY, scale, game.playerColor);
        }

        foreach(Unit u in game.enemies)
        {
            if (u.IsDead)
                continue;
            drawCircle(u.XY, scale, game.enemyColor);
        }
    }

    private void drawCircle(Vector2 pos, int diameter, Color color)
    {
        for(int i = 0; i < diameter; i++)
        {
            for(int j = 0; j < diameter; j++)
            {
                float x = i - (float) diameter / 2;
                float y = j - (float)diameter / 2;
                if (Mathf.Sqrt(x * x + y * y) < diameter / 2)
                {
                    Color c = minimap.GetPixel(i + (int)pos.x, j + (int)pos.y);
                    minimap.SetPixel(i + (int)pos.x, j + (int)pos.y, (c + color) / 2);
                }
            }
        }
    }
}

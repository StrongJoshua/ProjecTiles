using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour {
	public Texture2D tileset;
	private Texture2D minimap;
	public MapGenerator map;
    private int scale;

    private void updateMap()
    {
        Rect r = GetComponent<RectTransform>().rect;
        scale = (int) r.width / map.SizeX;

        minimap = new Texture2D(map.SizeX * scale, map.SizeY * scale, TextureFormat.ARGB4444, false);
        minimap.filterMode = FilterMode.Point;

        int x = 0;
        int y = 0;
        List<List<int>> tiles = map.readText();
        foreach (List<int> arr in tiles)
        {
            foreach (int k in arr)
            {
                Color c = tileset.GetPixel(k % tileset.width, tileset.height - 1 - k / tileset.width);
                for(int i = 0; i < scale; i++)
                {
                    for(int j = 0; j < scale; j++)
                    {
                        minimap.SetPixel(x * scale + i, map.SizeY * scale - scale - y * scale + j, c);
                    }
                }
                x++;
            }
            y++;
            x = 0;
        }
    }

    private void updateCharacters(IEnumerable<Unit> units, Color color)
    {
        foreach(Unit u in units)
        {
            if (u.IsDead)
                continue;
            drawCircle(u.XY, scale, color);
        }
    }

    public void refresh(GameManager game)
    {
        updateMap();
        updateCharacters(game.player.units, game.playerColor);
        updateCharacters(game.enemies, game.enemyColor);

        minimap.Apply();
        GetComponent<RawImage>().texture = minimap;
    }

    private void drawCircle(Vector2 pos, int diameter, Color color)
    {
        for(int i = 0; i < diameter; i++)
        {
            for(int j = 0; j < diameter; j++)
            {
                float x = i - (float) diameter / 2;
                float y = j - (float) diameter / 2;
                if (Mathf.Sqrt(x * x + y * y) < diameter / 2)
                {
                    Color c = minimap.GetPixel(i + (int)pos.x * scale, j + (int)pos.y * scale);
                    minimap.SetPixel(i + (int)pos.x * scale, j + (int)pos.y * scale, (c + color) / 2);
                }
            }
        }
    }
}

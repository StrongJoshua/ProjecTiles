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

        GetComponent<RectTransform>().sizeDelta = new Vector2(map.SizeX * scale, map.SizeY * scale);

        minimap = new Texture2D(map.SizeX * scale, map.SizeY * scale, TextureFormat.ARGB4444, false);
        minimap.filterMode = FilterMode.Point;

        Tile[,] tiles = map.Tiles;
        for(int y = 0; y < map.SizeY; y++)
        {
            for(int x = 0; x < map.SizeX; x++)
            {
                Tile t = tiles[x, y];
                int k = (int)t.Type;
                if (t.Type == Tile.TileType.boulder || t.Type == Tile.TileType.explosive)
                {
                    if (map.tileObjects[x, y].GetComponent<TileManager>().Destroyed)
                        k = (int)Tile.TileType.plain;
                }
                Color c = tileset.GetPixel(k % tileset.width, tileset.height - 1 - k / tileset.width);

                for (int i = 0; i < scale; i++)
                {
                    for (int j = 0; j < scale; j++)
                    {
                        minimap.SetPixel(x * scale + i, y * scale + j, c);
                    }
                }
            }
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

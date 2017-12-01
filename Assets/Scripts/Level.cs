using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {
    public TextAsset map;
    public Vector2[] playerSpawns = new Vector2[5];
    public int enemyCount;
    public Rect[] enemySpawnAreas;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public MapGenerator map;
    public int enemyCount;
    public GameObject[] unitTypes;

    private Unit[] enemies;

	// Use this for initialization
	void Start () {
        enemies = new Unit[enemyCount];
        for(int i = 0; i < enemyCount; i++)
        {
            enemies[i] = Instantiate(unitTypes[Random.Range(0, unitTypes.Length)], Vector3.zero, Quaternion.identity).GetComponent<Unit>();
            bool isValid = false;
            while (!isValid)
            {
                int tileX = Random.Range(0, map.SizeX);
                int tileY = Random.Range(0, map.SizeY);
                if (!map.GetTile(tileX, tileY).AllowsSpawn)
                    continue;
                foreach (Unit u in enemies)
                    if (u != null && u.X == tileX && u.Y == tileY)
                        continue;
                enemies[i].X = tileX;
                enemies[i].Y = tileY;
                isValid = true;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

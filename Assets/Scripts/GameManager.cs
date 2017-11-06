using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public MapGenerator map;
    public int enemyCount;
    public GameObject[] unitTypes;

    private GameObject[] enemies;

	// Use this for initialization
	void Start () {
        enemies = new GameObject[enemyCount];
        for(int i = 0; i < enemyCount; i++)
        {
            Vector3 position = new Vector3(Random.Range(0, map.SizeX) * map.step, .5f, Random.Range(0, map.SizeY) * map.step);
            enemies[i] = Instantiate(unitTypes[Random.Range(0, unitTypes.Length)], position, Quaternion.identity);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

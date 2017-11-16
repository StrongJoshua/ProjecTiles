using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public MapGenerator map;
    public GameManager manager;
    public int unitCount;

    private Dictionary<int, Dictionary<int, Unit>> defaultUnits;
	
	// Update is called once per frame
	void Update () {
		
	}
}

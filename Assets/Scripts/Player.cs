using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {

    public List<Unit> units;
    public HUDManager hud;

    public Player(Unit[] units)
    {
        this.units = new List<Unit>(units);
    }

    public void killUnit(Unit unit)
    {
        Debug.Log("Killing unit");
        units.Remove(unit);
        hud.removeUnit(unit);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

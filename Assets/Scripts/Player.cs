using System;
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
        units.Remove(unit);
        hud.removeUnit(unit);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    internal void placeUnits(Vector2[] playerSpawns)
    {
        for(int i = 0; i < units.Count; i++)
        {
            units[i].X = (int) playerSpawns[i].x;
            units[i].Y = (int) playerSpawns[i].y;
        }
    }

    internal int unitCount()
    {
        return units.Count;
    }
}

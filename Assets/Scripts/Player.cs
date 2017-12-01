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
        foreach (Unit u in this.units)
            u.player = this;
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

    internal void placeUnits(Vector2[] playerSpawns, Unit[,] characters)
    {
        for(int i = 0; i < units.Count; i++)
        {
            Unit unit = units[i];
            characters[unit.X, unit.Y] = null;
            unit.X = (int) playerSpawns[i].x;
            unit.Y = (int) playerSpawns[i].y;
            characters[unit.X, unit.Y] = unit;
        }
    }

    internal int unitCount()
    {
        return units.Count;
    }
}

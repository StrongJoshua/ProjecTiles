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

	public Player(List<Unit> otherUnits) {
		this.units = new List<Unit> ();
		foreach (Unit unit in otherUnits) {
			Unit newUnit = unit;
			newUnit.player = this;
			this.units.Add (newUnit);
		}
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

    internal void placeUnits(Vector2[] playerSpawns, Unit[,] characters, Transform parent, GameManager gm)
    {
        for(int i = 0; i < units.Count; i++)
        {
            Unit unit = units[i];
            characters[unit.X, unit.Y] = null;
            unit.X = (int) playerSpawns[i].x;
            unit.Y = (int) playerSpawns[i].y;
            characters[unit.X, unit.Y] = unit;
            unit.transform.parent = parent;
            unit.gameManager = gm;
        }
    }

    internal int unitCount()
    {
        return units.Count;
    }
}

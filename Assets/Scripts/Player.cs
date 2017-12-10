using System;
using System.Collections.Generic;
using UnityEngine;

public class Player {

    public List<Unit> units;
    public HUDManager hud;
    private List<Dictionary<String, float>> confirmedStats;

    public Player(Unit[] units)
    {
        this.units = new List<Unit>(units);
        foreach (Unit u in this.units)
        {
            u.player = this;
        }
        confirmStats();
    }

    public void killUnit(Unit unit)
    {
        hud.removeUnit(unit);
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

    internal int unitsAliveCount()
    {
        return units.FindAll((unit) => !unit.IsDead).Count;
    }

    internal bool hasUnitsAlive()
    {
        return unitsAliveCount() > 0;
    }

    internal void confirmStats()
    {
        confirmedStats = new List<Dictionary<string, float>>();
        units.RemoveAll((unit) => unit.IsDead);
        foreach(Unit u in units)
        {
            Dictionary<string, float> stats = u.getHiddenStats();
            stats["level"] = u.Level;
            confirmedStats.Add(stats);
        }
    }

    internal void resetStats()
    {
        for(int i = 0; i < units.Count; i++)
        {
            units[i].setHiddenStats((int) confirmedStats[i]["level"], confirmedStats[i]);
            units[i].resetHealthAP();
            units[i].gameObject.SetActive(true);
            if (units[i].anim != null)
                units[i].anim.SetTrigger("cancel");
            if (units[i].IsMedic)
                units[i].GetComponent<Medic>().resurrect();
            units[i].lookAt(units[i].XY);
        }
    }

    internal void removeParent()
    {
        foreach (Unit u in units)
            u.transform.parent = null;
    }
}

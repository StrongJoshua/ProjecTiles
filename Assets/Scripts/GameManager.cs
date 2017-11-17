using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public MapGenerator map;
    public int enemyCount, playerUnitCount;
    public GameObject[] unitTypes;

    public Color playerColor, enemyColor;

    // map tile x and y => which unit is occupying tile
    public Unit[,] characters;

    private Unit[] enemies;
    public Unit[] playerUnits;

    public Transform enemiesContainer;
    public Transform playerContainer;

    private Dictionary<Unit, List<Vector2>> pathManager;

    private Queue<Action> actions;

    private bool hasUpdate;
    public bool HasUpdate
    {
        get
        {
            if (hasUpdate)
            {
                hasUpdate = false;
                return true;
            }
            else
                return false;
        }
    }

    private EnemyAI ai;
    public bool AI;
    public float AIDelay;

	// Use this for initialization
	void Start () {
        enemies = new Unit[enemyCount];
        characters = new Unit[map.SizeX, map.SizeY];
        pathManager = new Dictionary<Unit, List<Vector2>>();

        enemies = generateUnits(enemiesContainer, enemyCount, enemyColor, Unit.Team.enemy);
        playerUnits = generateUnits(playerContainer, playerUnitCount, playerColor, Unit.Team.player);
        hasUpdate = false;

        actions = new Queue<Action>();

        ai = new EnemyAI(this, enemies, playerUnits, AIDelay);
	}

    Unit[] generateUnits(Transform container, int count, Color color, Unit.Team team)
    {
        Unit[] units = new Unit[count];
        for (int i = 0; i < count; i++)
        {
            GameObject unitType = unitTypes[UnityEngine.Random.Range(0, unitTypes.Length)];
            GameObject newObj = Instantiate(unitType, unitType.transform.position + new Vector3(0, .5f, 0), Quaternion.identity);
            newObj.transform.parent = container;
            Unit newUnit = newObj.GetComponent<Unit>();
            newUnit.team = team;
            GenerationUtils.setColor(newObj, color);

            addUnit(newUnit);
            units[i] = newUnit;
        }
        return units;
    }

    public void addUnit(Unit unit)
    {
        int tileX = UnityEngine.Random.Range(0, map.SizeX);
        int tileY = UnityEngine.Random.Range(0, map.SizeY);


        while (!map.GetTile(tileX, tileY).AllowsSpawn || characters[tileX, tileY] != null)
        {
            tileX = UnityEngine.Random.Range(0, map.SizeX);
            tileY = UnityEngine.Random.Range(0, map.SizeY);
        }

        unit.X = tileX;
        unit.Y = tileY;
        characters[tileX, tileY] = unit;

        unit.gameManager = this;
    }

    public Unit unitAt(int x, int y)
    {
        return characters[x, y];
    }

    public void moveSelectedUnit(int selectedX, int selectedY, int x, int y)
    {
        Unit unit = characters[selectedX, selectedY];

        if (unit == null || unit.team == Unit.Team.enemy || characters[x, y] != null)
            return;

        unit.moveTo(x, y);


        characters[selectedX, selectedY] = null;
        characters[x, y] = unit;
    }

    public void moveUnitOnPath(Unit unit, List<Vector2> path)
    {
        if (characters[(int)path[0].x, (int)path[0].y] != null)
        {
            unit.startle();
            return;
        }
        unit.setTarget(path[0]);
        characters[unit.X, unit.Y] = null;
        characters[(int)path[0].x, (int)path[0].y] = unit;
        pathManager.Add(unit, path);
    }

    internal void movementCallback(Unit unit)
    {
        List<Vector2> path = pathManager[unit];
        unit.costAP(unit.isFlying ? 1 : map.Tiles[unit.X, unit.Y].MovementCost);
        path.RemoveAt(0);
        hasUpdate = true;
        if (path.Count == 0)
        {
            pathManager.Remove(unit);
            return;
        }
        if(characters[(int)path[0].x, (int)path[0].y] != null)
        {
            unit.startle();
            pathManager.Remove(unit);
            return;
        }
        unit.setTarget(path[0]);
        characters[unit.X, unit.Y] = null;
        characters[(int)path[0].x, (int)path[0].y] = unit;
    }

    internal void apCallback(Unit unit)
    {
        hasUpdate = true;
    }

    internal void CallOnMainThread(Action action)
    {
        actions.Enqueue(action);
    }

    private void Update()
    {
        while (actions.Count > 0)
            actions.Dequeue().Invoke();
        if(AI)
            ai.think();
    }

    internal void deathCallback(Unit unit)
    {
        characters[unit.X, unit.Y] = null;
    }
}

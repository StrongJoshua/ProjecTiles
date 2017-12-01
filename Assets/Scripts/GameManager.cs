﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	private XMLParser xmlParser;
    public GameObject[] levels;
    private Level[] levelData;
    public int currentLevel;
    public MapGenerator mapGenerator;
    public GameObject[] unitTypes;

    public Color playerColor, enemyColor;

    // map tile x and y => which unit is occupying tile
    public Unit[,] characters;

    private Unit[] enemies;

    public Player player;

    public Transform enemiesContainer;
    public HUDManager hud;
    public Transform playerContainer;

    private Dictionary<Unit, List<Vector2>> pathManager;

    private Queue<Action> actions;

	private ArrayList unitBaseStats;

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

    internal Action<Unit> controlDeathCallback;

	void Awake() {
		xmlParser = new XMLParser ();
		unitBaseStats = new ArrayList ();

		foreach (GameObject unitType in unitTypes) {
			unitBaseStats.Add (xmlParser.getBaseStats (unitType.name));
		}
	}

	// Use this for initialization
	void Start () {
        currentLevel = 0;
        levelData = new Level[levels.Length];
        for (int i = 0; i < levels.Length; i++)
            levelData[i] = levels[i].GetComponent<Level>();

        mapGenerator.map = levelData[currentLevel].map;
        mapGenerator.generateMap();

        enemies = new Unit[levelData[currentLevel].enemyCount];
        characters = new Unit[mapGenerator.SizeX, mapGenerator.SizeY];
        pathManager = new Dictionary<Unit, List<Vector2>>();
        
        player = new Player(generateUnits(playerContainer, levelData[currentLevel].playerSpawns.Length, playerColor, Unit.Team.player));
        player.placeUnits(levelData[currentLevel].playerSpawns);
        player.hud = hud;

        enemies = generateUnits(enemiesContainer, levelData[currentLevel].enemyCount, enemyColor, Unit.Team.enemy);

        hasUpdate = false;

        actions = new Queue<Action>();

        ai = new EnemyAI(this, enemies, player.units.ToArray(), AIDelay);

        hud.initialize(player);
	}

    Unit[] generateUnits(Transform container, int count, Color color, Unit.Team team)
    {
        Unit[] units = new Unit[count];
        for (int i = 0; i < count; i++)
        {
			int randIndex = UnityEngine.Random.Range (0, unitTypes.Length);
            GameObject unitType = unitTypes[randIndex];

			Dictionary<string, float> Stats = (Dictionary<string, float>) unitBaseStats[randIndex];
            
			Unit newUnit = createUnit (unitType, Stats, container, color, team);
            addUnit(newUnit);
            units[i] = newUnit;
        }
        return units;
    }

	public Unit createUnit(GameObject unitType, Dictionary<string, float> Stats, Transform container, Color color, Unit.Team team) {
		GameObject newObj = Instantiate(unitType, unitType.transform.position + new Vector3(0, .5f, 0), Quaternion.identity);
		newObj.transform.parent = container;

		Unit newUnit = newObj.GetComponent<Unit>();
		newUnit.team = team;

		newUnit.setStats (Stats);
		newUnit.setGrowthRates (xmlParser.growthRates);
		newUnit.resetHealthAP ();

		GenerationUtils.setColor(newObj, color);

		return newUnit;
	}



    public void addUnit(Unit unit)
    {
        int tileX = UnityEngine.Random.Range(0, mapGenerator.SizeX);
        int tileY = UnityEngine.Random.Range(0, mapGenerator.SizeY);


        while (!mapGenerator.GetTile(tileX, tileY).AllowsSpawn || characters[tileX, tileY] != null)
        {
            tileX = UnityEngine.Random.Range(0, mapGenerator.SizeX);
            tileY = UnityEngine.Random.Range(0, mapGenerator.SizeY);
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
        unit.costAP(unit.isFlying ? 1 : mapGenerator.Tiles[unit.X, unit.Y].MovementCost);
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

    internal void uiCallback(Unit unit)
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
        if(controlDeathCallback != null)
            controlDeathCallback(unit);
    }

    public bool playerUnitsAlive()
    {
        return player.units.Count > 0;
    }

    internal Unit[] getOpponents(Unit.Team team)
    {
        if (team == Unit.Team.player)
            return enemies;
        else
            return player.units.ToArray();
    }

    internal Tile.TileType getTileTypeFor(Unit unit)
    {
        return mapGenerator.GetTileType(unit.X, unit.Y);
    }
}

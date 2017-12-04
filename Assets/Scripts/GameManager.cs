﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	private XMLParser xmlParser;
    public GameObject[] levels;
    private Level[] levelData;
    private int currentLevel;
	private GameObject persistentInfoInstance;
	private PersistentInfo persistentInfo;
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
    public bool debug;

    internal Action<Unit> controlDeathCallback;

	void Awake() {
		xmlParser = new XMLParser ();
		unitBaseStats = new ArrayList ();
		persistentInfoInstance = GameObject.Find ("Persistent Info");
		persistentInfo = persistentInfoInstance.GetComponent<PersistentInfo> ();

		foreach (GameObject unitType in unitTypes) {
			unitBaseStats.Add (xmlParser.getBaseStats (unitType.name));
		}
		currentLevel = persistentInfo.currentLevel;
	}

	// Use this for initialization
	void Start () {
        levelData = new Level[levels.Length];
        for (int i = 0; i < levels.Length; i++)
            levelData[i] = levels[i].GetComponent<Level>();

        initializeLevel(currentLevel);
	}

    private void OnValidate()
    {
        if (ai != null)
        {
            ai.debug = debug;
            ai.delay = AIDelay;
        }
    }

    public void retry() {
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	public void nextLevel() {
		int nextLevel = currentLevel + 1;
		if (nextLevel < levels.Length) {
			persistentInfo.currentLevel = nextLevel;
			//persistentInfo.currentPlayer = new Player (player.units);
			retry ();
		}
	}

	public void gameOver() {
		UserControl uC = GameObject.Find ("UserControl").GetComponent<UserControl> ();
		uC.gameOver ();
	}

	public void victory() {
		UserControl uC = GameObject.Find ("UserControl").GetComponent<UserControl> ();
		uC.victory ();
	}

	void initializeLevel(int currentLevel) {
		mapGenerator.map = levelData[currentLevel].map;
		mapGenerator.generateMap();

		enemies = new Unit[levelData[currentLevel].enemyCount];
		characters = new Unit[mapGenerator.SizeX, mapGenerator.SizeY];
		pathManager = new Dictionary<Unit, List<Vector2>>();

		GameObject createTeamMenu = GameObject.Find("CreateTeamMenu");
		if (createTeamMenu == null) {
			player = new Player (generateUnits (playerContainer, levelData [currentLevel], playerColor, Unit.Team.player));
			//persistentInfo.currentPlayer = new Player (player.units);
		} else {
			player = createTeamMenu.GetComponent<CreateTeamManager> ().generatePlayer (playerColor);
			//persistentInfo.currentPlayer = new Player (player.units);
		}
		player.placeUnits(levelData[currentLevel].playerSpawns, characters, playerContainer, this);
		player.hud = hud;

		enemies = generateUnits(enemiesContainer, levelData[currentLevel], enemyColor, Unit.Team.enemy);

        GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManager>().setEnemyCount(enemies.Length);

		hasUpdate = false;

		actions = new Queue<Action>();

		ai = new EnemyAI(this, enemies, player.units.ToArray(), AIDelay);
        ai.debug = debug;

		hud.initialize(player);
	}

    Unit[] generateUnits(Transform container, Level level, Color color, Unit.Team team)
    {
        Unit[] units = new Unit[team == Unit.Team.player ? level.playerSpawns.Length : level.enemyCount];
        for (int i = 0; i < units.Length; i++)
        {
			int randIndex = UnityEngine.Random.Range (0, unitTypes.Length);
            //randIndex = 3;
            GameObject unitType = unitTypes[randIndex];

			Dictionary<string, float> Stats = (Dictionary<string, float>) unitBaseStats[randIndex];
            
			Unit newUnit = createUnit (unitType, Stats, container, color, team, xmlParser);
            addUnit(newUnit, level.enemySpawnAreas);
            units[i] = newUnit;
        }
        return units;
    }

	public static Unit createUnit(GameObject unitType, Dictionary<string, float> Stats, Transform container, Color color, Unit.Team team, XMLParser xmlParser) {
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



    public void addUnit(Unit unit, Rect[] spawnAreas)
    {
        Rect area = spawnAreas[UnityEngine.Random.Range(0, spawnAreas.Length)];
        int tileX = UnityEngine.Random.Range((int) area.x, (int) (area.x + area.width));
        int tileY = UnityEngine.Random.Range((int) area.y, (int) (area.y + area.height));

        while (!mapGenerator.GetTile(tileX, tileY).AllowsSpawn || characters[tileX, tileY] != null)
        {
            tileX = UnityEngine.Random.Range((int)area.x, (int)(area.x + area.width));
            tileY = UnityEngine.Random.Range((int)area.y, (int)(area.y + area.height));
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
		if (!playerUnitsAlive ())
			gameOver ();
		if (!enemiesAlive ())
			victory ();
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

	public bool enemiesAlive() {
		return enemiesContainer.childCount > 0;
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

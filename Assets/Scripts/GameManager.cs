using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public MapGenerator map;
    public int enemyCount, playerUnitCount;
    public GameObject[] unitTypes;
    public Player player;

    public Color playerColor, enemyColor;

    // map tile x and y => which unit is occupying tile
    public Unit[,] characters;

    private Unit[] enemies;
    public Unit[] playerUnits;

    public Transform enemiesContainer;
    public Transform playerContainer;

    private Dictionary<Unit, List<Vector2>> pathManager;

    private bool update = false;

	// Use this for initialization
	void Start () {
        enemies = new Unit[enemyCount];
        characters = new Unit[map.SizeX, map.SizeY];
        pathManager = new Dictionary<Unit, List<Vector2>>();

        enemies = generateUnits(enemiesContainer, enemyCount, enemyColor, Unit.Team.enemy);
        playerUnits = generateUnits(playerContainer, enemyCount, playerColor, Unit.Team.player);
	}

    Unit[] generateUnits(Transform container, int count, Color color, Unit.Team team)
    {
        Unit[] units = new Unit[count];
        for (int i = 0; i < count; i++)
        {
            GameObject newObj = Instantiate(unitTypes[Random.Range(0, unitTypes.Length)], Vector3.zero, Quaternion.identity);
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
        int tileX = Random.Range(0, map.SizeX);
        int tileY = Random.Range(0, map.SizeY);


        while (!map.GetTile(tileX, tileY).AllowsSpawn || characters[tileX, tileY] != null)
        {
            tileX = Random.Range(0, map.SizeX);
            tileY = Random.Range(0, map.SizeY);
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
        unit.setTarget(path[0]);
        pathManager.Add(unit, path);
    }

    public void movementCallback(Unit unit)
    {
        List<Vector2> path = pathManager[unit];
        characters[unit.X, unit.Y] = null;
        unit.X = (int)path[0].x;
        unit.Y = (int)path[0].y;
        characters[unit.X, unit.Y] = unit;
        unit.costAP(map.Tiles[unit.X, unit.Y].MovementCost);
        path.RemoveAt(0);
        if (path.Count == 0)
        {
            pathManager.Remove(unit);
            update = true;
            return;
        }
        unit.setTarget(path[0]);
    }

    public bool updateAvailable()
    {
        bool b = update;
        update = false;
        return b;
    }
}

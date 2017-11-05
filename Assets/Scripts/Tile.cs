using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
    public enum TileType
    {
        plain,
        water,
        hill,
        swamp,
        forest,
		unknown
    }

    public static readonly Tile Plain = new Tile(1);
    public static readonly Tile Water = new Tile(4);
    public static readonly Tile Hill = new Tile(3);
    public static readonly Tile Swamp = new Tile(2);
    public static readonly Tile Forest = new Tile(2);

    private int movementCost;
    public int MovementCost
    {
        get { return movementCost; }
    }

    Tile(int movementCost)
    {
        this.movementCost = movementCost;
    }

    public static Tile GetTile(int type)
    {
        switch((TileType)type)
        {
            case TileType.plain: return Plain;
            case TileType.water: return Water;
            case TileType.hill: return Hill;
            case TileType.swamp: return Swamp;
            case TileType.forest: return Forest;
            default: return null;
        }
    }
}

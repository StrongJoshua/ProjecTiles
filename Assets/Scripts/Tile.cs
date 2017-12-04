﻿using System.Collections;
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
        sand,
        boulder,
        explosive,
		unknown
    }

    public static readonly Tile Plain = new Tile(TileType.plain, 2, true);
    public static readonly Tile Water = new Tile(TileType.water, 8, false);
    public static readonly Tile Hill = new Tile(TileType.hill, 6, false);
    public static readonly Tile Swamp = new Tile(TileType.swamp, 4, true);
    public static readonly Tile Forest = new Tile(TileType.forest, 4, true);
    public static readonly Tile Sand = new Tile(TileType.sand, 3, true);
    public static readonly Tile Boulder = new Tile(TileType.boulder, 2, false, true);
    public static readonly Tile Explosive = new Tile(TileType.explosive, 2, false, true);

    private TileType type;
    public TileType Type
    {
        get { return type; }
    }

    private int movementCost;
    public int MovementCost
    {
        set { movementCost = value;  }
        get { return movementCost; }
    }

    private bool allowsSpawn;
    public bool AllowsSpawn
    {
        get { return allowsSpawn; }
    }

    private bool impassable;
    public bool Impassable
    {
        get { return impassable; }
        set { impassable = value; }
    }

    Tile(TileType type, int movementCost, bool allowsSpawn)
    {
        this.type = type;
        this.movementCost = movementCost;
        this.allowsSpawn = allowsSpawn;
    }

    Tile(TileType type, int movementCost, bool allowsSpawn, bool impassable)
        : this (type, movementCost, allowsSpawn)
    {
        this.impassable = impassable;
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
            case TileType.sand: return Sand;
            case TileType.boulder: return Boulder;
            case TileType.explosive: return Explosive;
            default: return null;
        }
    }
}

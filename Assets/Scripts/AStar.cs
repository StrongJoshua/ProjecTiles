using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour {
    public static bool[,] movementMatrix(int ap, Tile[,] tiles, int x, int y)
    {
        bool[,] matrix = new bool[tiles.GetLength(0), tiles.GetLength(1)];
        recurseSearch(ap + tiles[x, y].MovementCost, tiles, x, y, matrix);
        return matrix;
    }

    private static void recurseSearch(int ap, Tile[,] tiles, int x, int y, bool[,] matrix)
    {
        if (x < 0 || y < 0 || x >= tiles.GetLength(0) || y >= tiles.GetLength(1))
            return;
        if (ap - tiles[x, y].MovementCost < 0)
            return;
        matrix[x, y] = true;
        ap -= tiles[x, y].MovementCost;
        recurseSearch(ap, tiles, x + 1, y, matrix);
        recurseSearch(ap, tiles, x - 1, y, matrix);
        recurseSearch(ap, tiles, x, y + 1, matrix);
        recurseSearch(ap, tiles, x, y - 1, matrix);
    }
}

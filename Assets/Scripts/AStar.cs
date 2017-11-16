using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour {
    public static bool[,] movementMatrix(int ap, Tile[,] tiles, int x, int y, bool isFlying)
    {
        bool[,] matrix = new bool[tiles.GetLength(0), tiles.GetLength(1)];
        recurseSearch(ap + (isFlying ? 1 : tiles[x, y].MovementCost), tiles, x, y, matrix, isFlying);
        return matrix;
    }

    private static void recurseSearch(int ap, Tile[,] tiles, int x, int y, bool[,] matrix, bool isFlying)
    {
        if (x < 0 || y < 0 || x >= tiles.GetLength(0) || y >= tiles.GetLength(1))
            return;
        if (ap - (isFlying ? 1 : tiles[x, y].MovementCost) < 0)
            return;
        matrix[x, y] = true;
        ap -= (isFlying ? 1 : tiles[x, y].MovementCost);
        recurseSearch(ap, tiles, x + 1, y, matrix, isFlying);
        recurseSearch(ap, tiles, x - 1, y, matrix, isFlying);
        recurseSearch(ap, tiles, x, y + 1, matrix, isFlying);
        recurseSearch(ap, tiles, x, y - 1, matrix, isFlying);
    }

    public static List<Vector2> AStarSearch(Tile[,] tiles, Vector2 start, Vector2 end)
    {
        Dictionary<Vector2, Vector2> parents = new Dictionary<Vector2, Vector2>();
        Dictionary<Vector2, int> g = new Dictionary<Vector2, int>();
        List<Vector2> open = new List<Vector2>();
        List<Vector2> closed = new List<Vector2>();
        open.Add(start);

        Vector2 nullVector = new Vector2(-1, -1);

        parents.Add(start, nullVector);
        g.Add(start, 0);

        while(open.Count > 0)
        {
            Vector2 current = getNext(open, end, tiles);

            if(current == end)
            {
                Vector2 parent = parents[end];
                List<Vector2> path = new List<Vector2>();
                path.Add(end);
                while(parent != nullVector)
                {
                    path.Insert(0, parent);
                    parent = parents[parent];
                }
                return path;
            }

            open.Remove(current);
            closed.Add(current);
            foreach(Vector2 neighbor in getNeighbors(current, tiles))
            {
                if (closed.Contains(neighbor)) continue;
                if(!open.Contains(neighbor))
                {
                    g.Add(neighbor, g[current] + tiles[(int)neighbor.x, (int)neighbor.y].MovementCost);
                    parents.Add(neighbor, current);
                    open.Add(neighbor);
                } else
                {
                    if(g[neighbor] > g[current] + tiles[(int)neighbor.x, (int)neighbor.y].MovementCost)
                    {
                        g[neighbor] = g[current] + tiles[(int)neighbor.x, (int)neighbor.y].MovementCost;
                        parents[neighbor] = current;
                    }
                }
            }
        }
        return new List<Vector2>();
    }

    private static Vector2 getNext(List<Vector2> open, Vector2 end, Tile[,] tiles)
    {
        Vector2 v = open[0];
        foreach (Vector2 v2 in open)
            v = heuristic(v, end) + tiles[(int)v.x, (int)v.y].MovementCost < heuristic(v2, end) + tiles[(int)v2.x, (int)v2.y].MovementCost ? v : v2;
        return v;
    }

    private static int heuristic(Vector2 pos, Vector2 end)
    {
        return (int)(Mathf.Abs(pos.x - end.x) + Mathf.Abs(pos.y - end.y));
    }

    private static List<Vector2> getNeighbors(Vector2 cur, Tile[,] tiles)
    {
        List<Vector2> l = new List<Vector2>();
        l.Add(new Vector2(cur.x - 1, cur.y));
        l.Add(new Vector2(cur.x + 1, cur.y));
        l.Add(new Vector2(cur.x, cur.y - 1));
        l.Add(new Vector2(cur.x, cur.y + 1));

        List<Vector2> r = new List<Vector2>();
        foreach(Vector2 v in l)
        {
            if (v.x >= 0 && v.y >= 0 && v.x < tiles.GetLength(0) && v.y < tiles.GetLength(1))
                r.Add(v);
        }
        return r;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI {
    private GameManager gameManager;
    private Tile[,] tiles;
    private Unit[] control, target;
    private float delay, lastAction;

	public EnemyAI(GameManager gameManager, Unit[] control, Unit[] target, float delay)
    {
        this.gameManager = gameManager;
        tiles = gameManager.mapGenerator.Tiles;
        this.control = control;
        this.target = target;
        this.delay = delay;
    }

    public void think()
    {
		if (Time.timeSinceLevelLoad - lastAction < delay )
            return;
        lastAction = Time.timeSinceLevelLoad;

        Unit cur = getNextControl();

        if (cur == null)
            return;
        Unit closest = getClosestTarget(cur);
        if(closest != null)
        {
            if(inRange(cur, closest))
            {
                cur.lookAt(closest.XY);
                cur.fire(false);
            } else
            {
                setStrategicDestination(cur, closest);
            }
        }
    }

    private Unit getNextControl()
    {
        Unit maxAPUnit = null;
        float ap = 0;
        foreach(Unit u in control)
        {
            if (u.IsDead || u.IsMoving)
                continue;
            if(u.AP > ap)
            {
                maxAPUnit = u;
                ap = u.AP;
            }
        }

        return maxAPUnit;
    }

    private Unit getClosestTarget(Unit unit)
    {
        Unit closest = null;
        float dist = float.PositiveInfinity;
        foreach(Unit t in target)
        {
            if (t.IsDead)
                continue;
            float d = Vector2.Distance(unit.XY, t.XY);
            if (d < dist)
            {
                closest = t;
                dist = d;
            }
        }

        return closest;
    }

    private bool inRange(Unit u, Unit u2)
    {
        return Vector2.Distance(u.XY, u2.XY) <= u.Projectile.range * .8F;
    }

    private void setStrategicDestination(Unit unit, Unit dest)
    {
        List<Vector2> path = stopAtRange(AStar.ConstrainPath(tiles, AStar.AStarSearch(tiles, unit.XY, dest.XY, unit.isFlying), (int)unit.AP), unit.Projectile.range * .8F, dest.XY);
        if (path.Count == 0)
            return;
        path = path.GetRange(1, path.Count - 1);
        if(path.Count > 0)
            gameManager.moveUnitOnPath(unit, path);
    }

    private List<Vector2> stopAtRange(List<Vector2> path, float range, Vector2 dest)
    {
        for(int i = 0; i < path.Count; i++)
        {
            if(Vector2.Distance(path[i], dest) <= range)
            {
                return path.GetRange(0, i + 1);
            }
        }
        return path;
    }
}

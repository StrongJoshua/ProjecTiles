using System.Collections.Generic;
using UnityEngine;

public class EnemyAI {
    private GameManager gameManager;
    private Tile[,] tiles;
    private List<Unit> controls, targets;
    private float lastAction;
    public float delay;
    public bool debug;

	public EnemyAI(GameManager gameManager, Unit[] control, Unit[] target, float delay)
    {
        this.gameManager = gameManager;
        tiles = gameManager.mapGenerator.Tiles;
        this.controls = new List<Unit>(control);
        this.targets = new List<Unit>(target);
        this.delay = delay;
    }

    public void think()
    {
		if (Time.timeSinceLevelLoad - lastAction < delay )
            return;
        controls.RemoveAll((unit) => unit.IsDead);
        targets.RemoveAll((unit) => unit.IsDead);
        lastAction = Time.timeSinceLevelLoad;

        Unit cur = getNextControl();

        if (cur == null)
            return;

        print("AI currently controlling " + cur.name + " at " + cur.XY);

        Unit target = getTarget(cur);
        if(target != null)
        {
            print("Found target " + target.name + " at " + target.XY);
            if(inRange(cur, target))
            {
                print("In range of target");
                if (!cur.canShoot())
                    return;
                print("Shot at target");
                cur.lookAt(target.XY);
                cur.fire(false);
            } else
            {
                print("Moving to target");
                setStrategicDestination(cur, target);
            }
        }
    }

    private Unit getNextControl()
    {
        Unit maxAPUnit = null;
        float ap = 0;
        Unit damaged = getDamagedTarget(controls[0]);
        foreach(Unit u in controls)
        {
            if (u.IsDead || u.IsMoving || (u.IsMedic && damaged == null))
                continue;
            if (inRange(u, getDamagedTarget(u)))
                continue;
            if(u.AP > ap)
            {
                maxAPUnit = u;
                ap = u.AP;
            }
        }

        return maxAPUnit;
    }

    private Unit getTarget(Unit unit)
    {
        if (unit.IsMedic)
            return getDamagedTarget(unit);
        else
            return getClosestTarget(unit);
    }

    private Unit getClosestTarget(Unit unit)
    {
        Unit closest = null;
        float dist = float.PositiveInfinity;
        foreach(Unit t in targets)
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

    private Unit getDamagedTarget(Unit unit)
    {
        Unit closest = null;
        float dist = float.PositiveInfinity;
        foreach(Unit u in controls)
        {
            if (u.IsDead || u.health == u.maxHealth)
                continue;
            float d = Vector2.Distance(unit.XY, u.XY);
            if (d < dist)
            {
                closest = u;
                dist = d;
            }
        }
        return closest;
    }

    private bool inRange(Unit u, Unit u2)
    {
        return Vector2.Distance(u.XY, u2.XY) <= (u.IsMedic ? u.GetComponent<Medic>().healRadius : u.Projectile.range * .8F);
    }

    private void setStrategicDestination(Unit unit, Unit dest)
    {
        float range = unit.IsMedic ? unit.GetComponent<Medic>().healRadius : unit.Projectile.range * .8F;

        List<Vector2> path = AStar.AStarSearch(tiles, unit.XY, dest.XY, unit.isFlying, (Unit[,])gameManager.characters.Clone());
        path = stopAtRange(AStar.ConstrainPath(tiles, path.GetRange(1, path.Count - 1), (int)unit.AP, unit.isFlying), range, dest.XY);
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

    private void print(string s)
    {
        if (debug)
            Debug.Log(s);
    }
}

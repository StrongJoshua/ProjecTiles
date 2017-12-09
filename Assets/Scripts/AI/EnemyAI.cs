using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI {
    private GameManager gameManager;
    private Tile[,] tiles;
    private List<Unit> controls, targets;
    private float lastAction;
    public float delay;
    public bool debug;

    private List<Aggro> aggroStates;

    private enum Aggro
    {
        move,
        moveClose,
        noMove
    }

	public EnemyAI(GameManager gameManager, Unit[] control, Unit[] target, float delay)
    {
        this.gameManager = gameManager;
        tiles = gameManager.mapGenerator.Tiles;
        this.controls = new List<Unit>(control);
        this.targets = new List<Unit>(target);
        this.delay = delay;

        this.aggroStates = new List<Aggro>();
        foreach (Unit u in controls)
            aggroStates.Add((Aggro) UnityEngine.Random.Range(2, Enum.GetValues(typeof(Aggro)).Length + 1));
    }

    public void think()
    {
		if (Time.timeSinceLevelLoad - lastAction < delay )
            return;

        removeDeads();
        lastAction = Time.timeSinceLevelLoad;

        Unit cur = getNextControl();

        if (cur == null)
            return;

        print("AI currently controlling " + cur.name + " at " + cur.XY);

        act(cur, aggroStates[controls.IndexOf(cur)]);
    }

    private Unit getNextControl()
    {
        print("Getting next control");
        if (controls.Count == 0)
            return null;

        Unit maxAPUnit = null;
        float ap = 0;
        Unit damaged = getDamagedTarget(controls[0]);
        foreach(Unit u in controls)
        {
            if (u.IsDead || u.IsMoving || (u.IsMedic && damaged == null))
                continue;

            Unit target = getTarget(u);

            if (u.IsMedic && inRange(u, target))
                continue;

            Aggro aggro = aggroStates[controls.IndexOf(u)];
            print(aggro + "");
            if (!u.IsMedic) {
                if (aggro == Aggro.noMove && Vector3.Distance(target.transform.position, u.transform.position) > u.Projectile.range * MapGenerator.step)
                    continue;
                else if (aggro == Aggro.moveClose && moveIters(u, target) > 1)
                    continue;
            }
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
            if (t.IsDead || gameManager.characters[t.X, t.Y] != t)
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

    private bool useEnvironment(Unit unit, Unit target)
    {
        List<GameObject> inRange = new List<GameObject>();
        foreach(GameObject tile in gameManager.mapGenerator.tileObjects)
        {
            TileManager tm = tile.GetComponent<TileManager>();
            if (tm == null || tm.Destroyed)
                continue;
            if(tm.DealsDamage)
            {
                BarrelManager bm = tile.GetComponent<BarrelManager>();
                if (Vector3.Distance(unit.transform.position, tile.transform.position) <= unit.Projectile.range * MapGenerator.step)
                {
                    if (Vector3.Distance(tile.transform.position, target.transform.position) <= bm.explodeRange * MapGenerator.step)
                    {
                        inRange.Add(tile);
                    }
                }
            }
        }

        if (inRange.Count == 0)
            return false;

        GameObject go = inRange[0];
        float max = Vector3.Distance(go.transform.position, unit.transform.position);
        
        for(int i = 1; i < inRange.Count; i++)
        {
            float dist = Vector3.Distance(inRange[i].transform.position, unit.transform.position);
            if(dist > max)
            {
                max = dist;
                go = inRange[i];
            }
        }

        unit.lookAt(go.transform.position / MapGenerator.step);
        unit.fire(false);

        return true;
    }

    private void removeDeads()
    {
        for (int i = controls.Count - 1; i >= 0; i--)
            if (controls[i].IsDead)
                aggroStates.RemoveAt(i);
        controls.RemoveAll((unit) => unit.IsDead);
        targets.RemoveAll((unit) => unit.IsDead);
    }

    private void act(Unit cur, Aggro aggro)
    {
        Unit target = getTarget(cur);
        if (target != null)
        {
            print("Found target " + target.name + " at " + target.XY);
            if (inRange(cur, target))
            {
                print("In range of target");
                if (!cur.canShoot())
                    return;
                print("Shot at target");
                cur.lookAt(target.XY);
                cur.fire(false);
            }
            else
            {
                if (useEnvironment(cur, target))
                {
                    print(cur.name + " shot at environment to harm " + target.name + " at " + target.XY);
                }
                else
                {
                    print("Moving to target");
                    setStrategicDestination(cur, target);
                }
            }
        }
    }

    private int moveIters(Unit unit, Unit dest)
    {
        float range = unit.IsMedic ? unit.GetComponent<Medic>().healRadius : unit.Projectile.range * .8F;

        List<Vector2> path = AStar.AStarSearch(tiles, unit.XY, dest.XY, unit.isFlying, (Unit[,])gameManager.characters.Clone());
        path = stopAtRange(AStar.ConstrainPath(tiles, path.GetRange(1, path.Count - 1), (int)unit.AP, unit.isFlying), range, dest.XY);

        int ap = 0;
        foreach (Vector2 v in path)
            ap += gameManager.mapGenerator.Tiles[(int)v.x, (int)v.y].MovementCost;
        return Mathf.CeilToInt(ap / (float) unit.maxAP);
    }
}

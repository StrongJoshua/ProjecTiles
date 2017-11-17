using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {
    private Unit[] control, target;
    private float delay, lastAction;

	public EnemyAI(Unit[] control, Unit[] target, float delay)
    {
        this.control = control;
        this.target = target;
        this.delay = delay;
    }

    public void think()
    {
        if (lastAction + delay > Time.timeSinceLevelLoad)
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
                cur.fire();
            }
        }
    }

    private Unit getNextControl()
    {
        Unit maxAPUnit = null;
        float ap = 0;
        foreach(Unit u in control)
        {
            if (u.IsDead)
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
        return Vector2.Distance(u.XY, u2.XY) <= u.Projectile.range;
    }
}

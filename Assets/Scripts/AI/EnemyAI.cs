using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {
    private Unit[] control, target;

	public EnemyAI(Unit[] control, Unit[] target)
    {
        this.control = control;
        this.target = target;
    }

    public void think()
    {
        foreach(Unit unit in control)
        {
            Unit closest = getClosestTarget(unit);
            if(closest != null)
            {

            }
        }
    }

    private Unit getClosestTarget(Unit unit)
    {
        Unit closest = null;
        float dist = float.PositiveInfinity;
        foreach(Unit t in target)
        {
            if (t == null)
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
}

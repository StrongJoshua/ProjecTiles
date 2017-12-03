using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BionadeEffect : ProjectileEffect {
    public int healAmount;

    public override void affect(Unit from, Unit to)
    {
        if (from.team == to.team)
            to.heal(healAmount);
    }
}

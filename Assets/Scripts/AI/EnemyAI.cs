using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {
    private Unit[] units;

	public EnemyAI(Unit[] units)
    {
        this.units = units;
    }

    public void think()
    {

    }
}

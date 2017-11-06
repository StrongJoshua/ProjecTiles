﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
	
	public int AP;
	public int health;
	public int x;
	public int y;

	public int maxAP;
	public int apChargeRate;
	public int perception;
	public int accuracy;
	public int defense;
	public int maxHealth;

	public float maxAPGrowth;
	public float apChargeRateGrowth;
	public float perceptionGrowth;
	public float accuracyGrowth;
	public float defenseGrowth;
	public float healthGrowth;

    public Team team;

	public enum Team
	{
		player,
		enemy
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void levelUp()
    {
        if (increase(maxAPGrowth))
            maxAP++;
        if (increase(apChargeRateGrowth))
            apChargeRate++;
        if (increase(perceptionGrowth))
            perception++;
        if (increase(accuracyGrowth))
            accuracy++;
        if (increase(defenseGrowth))
            defense++;
        if (increase(healthGrowth))
            health++;
    }

    private bool increase(float growth)
    {
        return Random.Range(0, 1f) <= growth;
    }
}

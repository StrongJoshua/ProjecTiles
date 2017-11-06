using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
	
	public int AP;
	public int health;
	public int x;
	public int y;

	public int basemaxAP;
	public int baseAPChargeRate;
	public int basePerception;
	public int baseAccuracy;
	public int baseDefense;
	public int baseMaxHealth;

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
}

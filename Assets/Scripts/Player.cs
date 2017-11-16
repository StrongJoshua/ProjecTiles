using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public MapGenerator map;
    public GameManager manager;
    public int unitCount;

    private Dictionary<int, Dictionary<int, Unit>> defaultUnits;

	// Use this for initialization
	public Unit[] initialize () {
        return instantiateUnits();
	}

    Unit[] instantiateUnits()
    {
        Unit[] unitList = new Unit[unitCount];
        for (int i = 0; i < unitCount; i++)
        {
            GameObject newUnitObject = Instantiate(manager.unitTypes[Random.Range(0, manager.unitTypes.Length)], Vector3.zero, Quaternion.identity);
            GenerationUtils.setColor(newUnitObject, manager.playerColor);

            unitList[i] = newUnitObject.GetComponent<Unit>();
            unitList[i].team = Unit.Team.player;
        }
        return unitList;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public Dictionary<int, Dictionary<int, Unit>> units;
    public Unit[] unitList;
    public MapGenerator map;
    public GameManager manager;
    public int unitCount;
    public GameObject[] unitTypes;

    private Dictionary<int, Dictionary<int, Unit>> defaultUnits;

	// Use this for initialization
	public void initialize () {
        units = new Dictionary<int, Dictionary<int, Unit>>();

        instantiateUnits();
	}

    void instantiateUnits()
    {
        unitList = new Unit[unitCount];
        for (int i = 0; i < unitCount; i++)
        {
            GameObject newUnitObject = Instantiate(unitTypes[Random.Range(0, unitTypes.Length)], Vector3.zero, Quaternion.identity);

            newUnitObject.GetComponentsInChildren<SkinnedMeshRenderer>()[0]
                .material.color = Color.blue;

            unitList[i] = newUnitObject.GetComponent<Unit>();
            unitList[i].team = Unit.Team.player;
            manager.addUnit(unitList[i]);
        } 
    }

    void addUnit(Unit unit)
    {
        if (units[unit.X] == null)
        {
            units[unit.X] = new Dictionary<int, Unit>();
        }
        units[unit.X][unit.Y] = unit;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

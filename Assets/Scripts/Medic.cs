using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medic : MonoBehaviour {
	public List<Collider> TriggerList;
	float currTime;
	float delay;
	// Use this for initialization
	void Start () {
		currTime = Time.timeSinceLevelLoad;
		delay = 2f;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.timeSinceLevelLoad - currTime > delay){
			currTime = Time.timeSinceLevelLoad;
		foreach (Collider c in TriggerList) {
			Unit unit = c.gameObject.GetComponent<Unit> ();
			if (unit != null) {
				unit.heal (1);
			}
		}
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if (!TriggerList.Contains (col))
			TriggerList.Add (col);
	}
	void OnTriggerExit(Collider col)
	{
		if (TriggerList.Contains (col))
			TriggerList.Remove (col);
	}
}

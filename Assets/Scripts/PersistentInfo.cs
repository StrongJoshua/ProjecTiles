using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentInfo : MonoBehaviour {

	static PersistentInfo instance;
	public int currentLevel;
	public Player currentPlayer;

	/*public struct PersistentStats {
		string name;
		float maxHP;
		float maxAP;
		float apChargeRate;
		float accuracy;
		float perception;
	}*/

	//List<PersistentStats> unitStatsList;
	// Use this for initialization
	void Awake () {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (gameObject);
		} else if (instance != this) {
			Destroy (gameObject);
		}
		//unitStatsList = new List <PersistentStats>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/*public void updateUnits(List<Unit> units) {
		foreach (Unit unit in units) {
			
		}
	}

	public void updateStats() {
		List<Unit> units = currentPlayer.units;
		foreach (Unit unit in units) {
			PersistentStats unitStats = new PersistentStats ();
		}
	}*/

}

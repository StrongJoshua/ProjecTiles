using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevPause : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha0))
			Time.timeScale = 0;
		if (Input.GetKeyDown (KeyCode.Alpha1))
			Time.timeScale = 1;
		
	}
}

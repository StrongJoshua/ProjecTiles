using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockTestPleaseDelete : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().AddForce(new Vector3(0f, 600f, 0f));
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

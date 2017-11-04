using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround (new Vector3(22.5f,0,22.5f), Vector3.up, 5 * Time.deltaTime);
		
	}
}

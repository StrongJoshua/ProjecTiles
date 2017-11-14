using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimRing : MonoBehaviour {
	public Camera camera;
	float lockX;
	float lockZ;
	// Use this for initialization
	void Start () {
		lockX = transform.rotation.x;
		lockZ = transform.rotation.z;
	}
	
	// Update is called once per frame
	void Update () {
		//Grab the current mouse position on the screen
		if (Input.GetKey (KeyCode.D)) {
			transform.Rotate (0,0,-70f * Time.deltaTime);
		}
		if (Input.GetKey (KeyCode.A)) {
			transform.Rotate (0,0,70f * Time.deltaTime);
		}

	
	}
}

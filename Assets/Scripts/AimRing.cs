using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimRing : MonoBehaviour {
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//Grab the current mouse position on the screen
		float InputAxis = (Input.GetButtonDown("keyAim")) ? Input.GetAxisRaw("keyAim") : Input.GetAxis("Aim");


		if (InputAxis > 0) {
			transform.Rotate (0, 240f * Time.deltaTime * InputAxis,0);
		}
		if (InputAxis < 0) {
			transform.Rotate (0, 240f * Time.deltaTime * InputAxis,0);
		}

	
	}
}

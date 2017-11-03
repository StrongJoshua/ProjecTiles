using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
	public Camera cam;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.UpArrow)) {
			cam.gameObject.transform.position += Vector3.forward;
		}
		else if (Input.GetKey (KeyCode.DownArrow)) {
			cam.gameObject.transform.position += Vector3.back;
		}
		else if (Input.GetKey (KeyCode.RightArrow)) {
			cam.gameObject.transform.position += Vector3.right;
		}
		else if (Input.GetKey (KeyCode.LeftArrow)) {
			cam.gameObject.transform.position += Vector3.left;
		}
		else if (Input.GetAxis("Mouse ScrollWheel") > 0 && cam.orthographicSize < 13) {
			cam.orthographicSize+=0.3f;
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0 && cam.orthographicSize > 2) {
			cam.orthographicSize-=0.3f;
		}

		
	}
}

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
		Vector3 mousePosition = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y, Input.mousePosition.z));

//		RaycastHit hit;
//		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//		if (Physics.Raycast (ray, out hit)) {
//			Debug.DrawLine (ray.origin, hit.point, Color.red);
//			//Rotates toward the mouse
//			Vector3 dir = (hit.point - transform.position).normalized;
//			transform.rotation = Quaternion.Euler (dir);
//		}

	
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudioCameraRotate : MonoBehaviour {
	public Transform point;
	public float speed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAround (point.position, Vector3.up, Time.deltaTime * speed);
		
	}
}

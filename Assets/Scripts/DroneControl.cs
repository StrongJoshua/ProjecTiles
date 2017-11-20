using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneControl : MonoBehaviour {
	public float velocity;
	public float turnRate;
	Rigidbody rigidbody;
	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody> ();
		rigidbody.velocity = transform.forward;
	}
	
	// Update is called once per frame
	void Update () {
		rigidbody.velocity = transform.forward * velocity;
		transform.Rotate(0, Input.GetAxis("Horizontal") * Time.deltaTime * turnRate,  0);
	}
}

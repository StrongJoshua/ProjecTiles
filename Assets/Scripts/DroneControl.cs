using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneControl : MonoBehaviour {
	float velocity;
	float rollRate;
	Rigidbody rigidbody;
	// Use this for initialization
	void Start () {
		rollRate = 1f;	
		velocity = 2f;
		rigidbody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate()
	{
		rigidbody.AddRelativeForce (transform.forward * velocity);
		rigidbody.AddRelativeTorque (transform.forward * -Input.GetAxis ("Horizontal") * rollRate);
		rigidbody.AddRelativeTorque (transform.right * Input.GetAxis ("Vertical") * rollRate);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneControl : MonoBehaviour {
	float velocity;
	float rollRate;
	Rigidbody rigidbody;
	// Use this for initialization
	void Start () {
		rollRate = 4f;	
		velocity = 10f;
		rigidbody = GetComponent<Rigidbody> ();
		rigidbody.velocity = transform.forward;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate()
	{
		rigidbody.velocity = transform.forward;
		rigidbody.AddRelativeForce (Vector3.up * velocity);
		rigidbody.AddRelativeTorque (transform.up * Input.GetAxis ("Horizontal") * rollRate);
		rigidbody.AddRelativeTorque (transform.right * Input.GetAxis ("Vertical") * rollRate);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour {
	public GameObject projectile;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.F)) {
			GameObject temp = Instantiate (projectile);
			temp.transform.position = transform.position + new Vector3 (0,1,0);
			temp.GetComponent<Rigidbody> ().AddForce (new Vector3(0,0,100));
		}
			
	}
}

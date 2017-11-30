using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
	public GameObject projectile;
	public Projectile projectileInfo;
	public float projectileSpeed;
	public 
	/*
	 * How accurate the projectile is, higher value decreases accuracy as
	 * it increases the possible spread of values of the projectiles initial vector path 
	*/
	//public float gunSpread;
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		/*if (Input.GetKeyDown (KeyCode.F) || Input.GetButtonDown("Fire1")) {
			int numToFire = projectile.GetComponent<Projectile> ().numToFire;
			for (int i = 0; i < numToFire; i++) {
				GameObject temp = Instantiate (projectile);
				temp.transform.rotation = transform.rotation;
				temp.transform.Rotate (new Vector3 (90, 0, 0));
				temp.transform.position = transform.position + transform.forward + transform.up;
				Vector3 aim = this.transform.forward * projectileSpeed;
				aim.x = aim.x + Random.Range (-gunSpread, gunSpread);
				temp.GetComponent<Rigidbody> ().AddForce (aim);
			}
		}*/
			
	}
		
}

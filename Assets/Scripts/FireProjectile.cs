using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
	public GameObject projectile;
	public float speed;
	/*
	 * How accurate the projectile is, higher value decreases accuracy as
	 * it increases the possible spread of values of the projectiles initial vector path 
	*/
	public float accuracySpread;
	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.F)) {
			if (projectile.GetComponent<Projectile> ().type == Projectile.ProjectileType.shotgun) {
				for (int i = 0; i < 10; i++) {
					GameObject temp = Instantiate (projectile);
					temp.transform.position = transform.position + new Vector3 (0, 1, 0);
					temp.GetComponent<Rigidbody> ().AddForce (new Vector3 (Random.Range (-accuracySpread, accuracySpread), 0, speed));
				}
			} else {
				GameObject temp = Instantiate (projectile);
				temp.transform.position = transform.position + new Vector3 (0, 1, 0);
				temp.GetComponent<Rigidbody> ().AddForce (new Vector3 (Random.Range (-accuracySpread, accuracySpread), 0, speed));
			}
		}
			
	}
}

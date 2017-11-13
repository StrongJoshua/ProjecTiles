using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
	public GameObject projectile;
	public float projectileSpeed;
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
//			int numToFire = projectile.GetComponent<Projectile> ().type == Projectile.ProjectileType.shotgun ? 5 : 1;
//			for (int i = 0; i < numToFire; i++) {
//				GameObject temp = Instantiate (projectile);
//				temp.transform.position = transform.position + new Vector3 (0, 1, 0);
//				temp.GetComponent<Rigidbody> ().AddForce (new Vector3 (Random.Range (-accuracySpread, accuracySpread), 0, speed));
//			}
			int numToFire = projectile.GetComponent<Projectile> ().type == Projectile.ProjectileType.shotgun ? 5 : 1;
			for (int i = 0; i < numToFire; i++) {
				GameObject temp = Instantiate (projectile);
				temp.transform.rotation = transform.rotation;
				temp.transform.Rotate (new Vector3 (90, 0, 0));
				temp.transform.position = transform.position + transform.forward + transform.up;
				Vector3 aim = this.transform.forward * projectileSpeed;
				aim.x = aim.x + Random.Range (-accuracySpread, accuracySpread);
				//print (aim.ToString ());
				temp.GetComponent<Rigidbody> ().AddForce (aim);
			}
		}
			
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	public float lifetime;
	public int maxDamage;
	int currDamage;
	float startTime;
	public int numToFire;
	public float speed;




	protected virtual void Awake () {
		startTime = Time.timeSinceLevelLoad;
		currDamage = maxDamage;

	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if (Time.timeSinceLevelLoad - startTime > lifetime) {
			
				Destroy (gameObject);
		
		}
	}

	void OnTriggerEnter()
	{
	}

	protected virtual void OnCollisionEnter(Collision col) {
		Unit hitUnit = col.gameObject.GetComponent<Unit> (); 
		
		if (hitUnit != null) {
			Debug.Log ("DING DING");
			hitUnit.takeDamage (currDamage);
		}

		Destroy (gameObject);
	}
		
}

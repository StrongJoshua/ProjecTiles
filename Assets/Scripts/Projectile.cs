using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	public float range;
	public int maxDamage;
	int currDamage;
	float startTime;
	public int numToFire;
	public float speed;
	public Unit.Team team;

    private Vector3 start;

	protected virtual void Awake () {
		startTime = Time.timeSinceLevelLoad;
		currDamage = maxDamage;
	}

    private void Start()
    {
        start = gameObject.transform.position;
    }

    // Update is called once per frame
    protected virtual void Update () {
		if (Vector3.Distance(start, gameObject.transform.position) >= range * MapGenerator.step) {
				Destroy (gameObject);
		}
	}

	void OnTriggerEnter(Collider col)
	{
		Unit hitUnit = col.gameObject.GetComponent<Unit> (); 

		if (hitUnit != null) {
			//Debug.Log ("DING DING");
			if(hitUnit.team != team)
				hitUnit.takeDamage (currDamage);
			Destroy (gameObject);
		}
	}

	protected virtual void OnCollisionEnter(Collision col) {
		Unit hitUnit = col.gameObject.GetComponent<Unit> (); 
		
		if (hitUnit != null) {
            //Debug.Log ("DING DING");
			if(hitUnit.team != team)
				hitUnit.takeDamage (currDamage);
			Destroy (gameObject);
		}
	}
		
}

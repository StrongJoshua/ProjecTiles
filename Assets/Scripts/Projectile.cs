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
	public bool explode;
	public float explodeRange;
	public GameObject explodeParticle;
	public GameObject origin;

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
		float distance = Vector3.Distance (start, gameObject.transform.position); 
		if (distance >= range * MapGenerator.step) {
			if (explode) {
				Instantiate (explodeParticle, transform.position, transform.rotation);
				Collider[] allColliders = Physics.OverlapSphere (transform.position, explodeRange);
				foreach (Collider c in allColliders) {
					Unit t = c.gameObject.GetComponent<Unit> ();
					if (t != null && !t.team.Equals(team) && c.gameObject != origin)
						t.takeDamage ((int)(currDamage * (distance/(range * MapGenerator.step))));
				}
				Destroy (gameObject);
			}
			else
				Destroy (gameObject);
		}
	}

	void OnTriggerEnter(Collider col)
	{
		float distance = Vector3.Distance (start, gameObject.transform.position); 
		Unit hitUnit = col.gameObject.GetComponent<Unit> (); 

		if(hitUnit != null && col.gameObject != origin) {
			if(!hitUnit.IsDead && hitUnit.team != team)
				hitUnit.takeDamage ((int)(currDamage * (distance/(range * MapGenerator.step))));
			Destroy (gameObject);
		}
	}

	protected virtual void OnCollisionEnter(Collision col) {
		Unit hitUnit = col.gameObject.GetComponent<Unit> (); 
		
		if (hitUnit != null) {
            //Debug.Log ("DING DING");
			if(!hitUnit.IsDead && hitUnit.team != team && col.gameObject != origin)
				hitUnit.takeDamage (currDamage);
			Destroy (gameObject);
		}
	}
		
}

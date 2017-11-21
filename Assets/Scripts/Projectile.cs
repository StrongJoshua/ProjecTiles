using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	public float range;
	public int maxDamage;
	float startTime;
	public int numToFire;
	public float speed;
	public Unit.Team team;
	public bool explodes;
	public float explodeRange;
	public GameObject explodeParticle;
	public GameObject origin;

    private Vector3 start;

	protected virtual void Awake () {
		startTime = Time.timeSinceLevelLoad;
	}

    private void Start()
    {
        start = gameObject.transform.position;
    }

    // Update is called once per frame
    protected virtual void Update () {
		float distance = Vector3.Distance (start, gameObject.transform.position); 
		if (distance >= range * MapGenerator.step) {
			if (explodes) {
                explode();
			}
			else
				Destroy (gameObject);
		}
	}

	void OnTriggerEnter(Collider col)
	{
		float distance = Vector3.Distance (start, gameObject.transform.position); 
		Unit hitUnit = col.gameObject.GetComponent<Unit> (); 

		if(hitUnit != null && col.gameObject != origin && !hitUnit.IsDead) {
            if (explodes)
                explode();
            else
            {
                if (hitUnit.team != team)
                    hitUnit.takeDamage((int)(maxDamage * (1 - (distance / (2 * range * MapGenerator.step))))); // multiply by 2 to min at half damage
                Destroy(gameObject);
            }
		}
	}

    private void explode()
    {
        Instantiate(explodeParticle, transform.position, transform.rotation);
        Collider[] allColliders = Physics.OverlapSphere(transform.position, explodeRange * MapGenerator.step);
        foreach (Collider c in allColliders)
        {
            Unit t = c.gameObject.GetComponent<Unit>();
            if (t != null && !t.team.Equals(team) && c.gameObject != origin)
            {
                t.takeDamage((int)(maxDamage * (1 - Vector3.Distance(t.gameObject.transform.position, this.gameObject.transform.position) / (explodeRange * MapGenerator.step))));
            }
        }
        Destroy(gameObject);
    }
}

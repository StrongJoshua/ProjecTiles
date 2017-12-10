using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsProjectile : MonoBehaviour {

	public int maxDamage;
	int curDamage;
	public float lifeTime;
	float currSpeed;
	float currTime;
	public GameObject origin;
	float startTime;
	void Awake() {
		
	}

	// Use this for initialization
	void Start () {
		startTime = Time.timeSinceLevelLoad;
	}
	
	// Update is called once per frame
	void Update () {
		currSpeed = GetComponent<Rigidbody> ().velocity.magnitude;
		if (currSpeed >= 10f)
			currSpeed = 10f;
		curDamage = (int)(currSpeed/10 * maxDamage);

		currTime = Time.timeSinceLevelLoad - startTime; 
		if (currTime > lifeTime) {
			Destroy (gameObject);
		}

	}

	private void OnCollisionEnter(Collision col)
	{
		OnTriggerEnter (col.collider);
		
	}

	private void OnTriggerEnter(Collider col) {
		Unit hitUnit = col.gameObject.GetComponent<Unit> ();
		TileManager tm = col.gameObject.GetComponentInParent<TileManager>();

		if (hitUnit != null && !hitUnit.IsDead)
		{
			hitUnit.takeDamage(curDamage);

		}

		if (tm != null && !tm.Destroyed)
			tm.hit(curDamage, gameObject);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneControl : MonoBehaviour {
	public float velocity;
	public float turnRate;
	public float lifetime;
	public GameObject explodeParticle;
	public Unit.Team team;
	public float explodeRange;
	public GameObject origin;
	public int maxDamage;
	public UserControl userControl;
	Vector3 offset;

	float startTime;
	Rigidbody rigidbody;
	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody> ();
		rigidbody.velocity = transform.forward;
		userControl.mapControl = false;
		startTime = Time.timeSinceLevelLoad;
		offset = new Vector3 (0, Camera.main.gameObject.transform.position.y - transform.position.y, Camera.main.gameObject.transform.position.z - transform.position.z );
	}
	void OnTriggerEnter(Collider col)
	{
		Unit hitUnit = col.gameObject.GetComponent<Unit> (); 
		if (hitUnit != null && col.gameObject != origin && !hitUnit.IsDead) {
			explode ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad - startTime > lifetime)
			explode ();
			
		rigidbody.velocity = transform.forward * velocity;
		transform.Rotate(0, Input.GetAxis("Horizontal") * Time.deltaTime * turnRate,  0);
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
		userControl.mapControl = true;
		Destroy(gameObject);
	}
	void LateUpdate()
	{
		Camera.main.gameObject.transform.position = transform.position + offset;
	}
}

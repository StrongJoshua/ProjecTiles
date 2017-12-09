using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneControl : MonoBehaviour
{
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
	bool exploded;
	public GameObject droneModel;
	public AudioClip explosion;
	public GameObject dronedot;

	float startTime;
	Rigidbody rigidbody;
	GameObject DroneDot;
	// Use this for initialization
	void Start ()
	{
		rigidbody = GetComponent<Rigidbody> ();
		rigidbody.velocity = transform.forward;
		userControl.mapControl = false;
		startTime = Time.timeSinceLevelLoad;
		offset = new Vector3 (0, Camera.main.gameObject.transform.position.y - transform.position.y, Camera.main.gameObject.transform.position.z - transform.position.z);
		exploded = false;
		DroneDot = Instantiate (dronedot, transform.position + transform.forward * explodeRange * MapGenerator.step, transform.rotation);
		DroneDot.transform.parent = transform;

	}

	void OnTriggerEnter (Collider col)
	{
		Unit hitUnit = col.gameObject.GetComponent<Unit> (); 
		if (hitUnit != null && col.gameObject != origin && !hitUnit.IsDead) {
			explode ();
		}
	}

    private void OnCollisionEnter(Collision collision)
    {
        TileManager tm = collision.gameObject.GetComponentInParent<TileManager>();
        if (tm != null && !tm.Destroyed)
            explode();
    }

    // Update is called once per frame
    void Update ()
	{
		userControl.mapControl = false;
		if (!exploded) {
			DoRenderer ();
			if (Time.timeSinceLevelLoad - startTime > lifetime) {
				explode ();
				startTime = Time.timeSinceLevelLoad;
			}
			rigidbody.velocity = transform.forward * velocity;
			transform.Rotate (0, Input.GetAxis ("Horizontal") * Time.deltaTime * turnRate, 0);
		} else {
			if (Time.timeSinceLevelLoad - startTime > 1f) {
				userControl.returnMapControl ();
				Destroy (gameObject);
			}
		}
	}

	private void explode ()
	{
		Instantiate (explodeParticle, transform.position, transform.rotation);
		AudioSource.PlayClipAtPoint (explosion, transform.position);
		Collider[] allColliders = Physics.OverlapSphere (transform.position, explodeRange * MapGenerator.step);
		foreach (Collider c in allColliders) {
            int damage = (int)(maxDamage * (1 - Vector3.Distance(c.gameObject.transform.position, this.gameObject.transform.position) / (2 * explodeRange * MapGenerator.step)));
            Unit t = c.gameObject.GetComponent<Unit> ();
			if (t != null && !t.team.Equals (team) && c.gameObject != origin) {
				t.takeDamage(damage);
			}

            TileManager tm = c.gameObject.GetComponent<TileManager>();
            if (tm != null && !tm.Destroyed)
                tm.hit(damage, gameObject);
		}
		//gameObject.SetActive (false);
		exploded = true;
		droneModel.SetActive (false);
		GetComponent<ParticleSystem> ().Stop ();
		gameObject.GetComponent<LineRenderer> ().enabled = false;
		startTime = Time.timeSinceLevelLoad;
		Destroy (DroneDot);
	}

	void LateUpdate ()
	{
		if (!exploded) {
			Camera.main.gameObject.transform.position = transform.position + offset;
			if (Mathf.Abs (Input.GetAxis ("Vertical")) > 0 || Mathf.Abs (Input.GetAxis ("Horizontal")) > 0) {
				//DroneDot.transform.RotateAround (transform.position, Vector3.up, Mathf.Atan (Input.GetAxis ("Vertical") / Input.GetAxis ("Horizontal")));
				float degrees = Mathf.Atan2 (Input.GetAxis ("Vertical") , Input.GetAxis ("Horizontal"));
				//print (degrees);
				DroneDot.transform.position = transform.position +  new Vector3 (explodeRange * MapGenerator.step * Mathf.Cos(degrees), 0, explodeRange * MapGenerator.step * Mathf.Sin(degrees));
			}
		}
	}


	public void DoRenderer ()
	{
		float radius = explodeRange * MapGenerator.step;
		int numSegments = 128;
		LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer> ();
		Color c1 = new Color (1, 0f, 0f, 1);
		Color c2 = new Color (1, .8f, 0, 1);

		Gradient gradient = new Gradient ();
		gradient.colorKeys = new GradientColorKey[] {
			new GradientColorKey (c1, 0),
			new GradientColorKey (c2, .5f),
			new GradientColorKey (c1, 1)
		};

		lineRenderer.material = new Material (Shader.Find ("Particles/Additive"));
		lineRenderer.colorGradient = gradient;
		lineRenderer.startWidth = .2f;
		lineRenderer.endWidth = .2f;
		lineRenderer.positionCount = numSegments + 1;
		lineRenderer.useWorldSpace = false;

		float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
		float theta = Mathf.PI / 2;

		for (int i = 0; i < numSegments + 1; i++) {
			float x = radius * Mathf.Cos (theta);
			float z = radius * Mathf.Sin (theta);
			Vector3 pos = new Vector3 (x, -1.5f, z);
			lineRenderer.SetPosition (i, pos);
			theta += deltaTheta;
		}
	}
}

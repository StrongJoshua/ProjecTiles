using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	public float lifetime;
	public float damage;
	public ProjectileType type;
	float startTime;

	public enum ProjectileType {
	laser,
	shotgun,
	slug
	}


	void Start () {
		startTime = Time.timeSinceLevelLoad;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad - startTime > lifetime) {
			if (type != ProjectileType.slug)
				Destroy (gameObject);
			else {
				Destroy (gameObject);
			}
		}
	}

	void OnTriggerEnter()
	{
	}
}

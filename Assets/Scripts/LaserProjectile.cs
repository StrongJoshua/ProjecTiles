using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserProjectile : Projectile {


	// Use this for initialization
	protected override void Awake () {
		base.Awake ();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();
	}

	protected override void OnCollisionEnter(Collision col) {
		base.OnCollisionEnter (col);
	}
}

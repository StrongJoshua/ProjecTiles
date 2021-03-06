﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
	public float range;
    public bool timed;
	public int maxDamage;
	float startTime;
	public int numToFire;
	public float speed;
	public bool damageFalloff, explodes;
	public float explodeRange;
	public GameObject explodeParticle;
	public Unit origin;
	public AudioClip explosion;
	public GameObject hitEffect;

    private ProjectileEffect projectileEffect;

    private Vector3 last;
    private float distanceTraveled;
	private AudioSource audio;

	protected virtual void Awake () {
        projectileEffect = GetComponent<ProjectileEffect>();
		startTime = Time.timeSinceLevelLoad;
	}

    private void Start()
    {
        last = gameObject.transform.position;
        distanceTraveled = 0;
		if (GetComponent<AudioSource> () != null) {
			audio = GetComponent<AudioSource> ();
			audio.volume = PersistentInfo.Instance ().SFXVolume;
		}
    }

    // Update is called once per frame
    protected virtual void Update () {
		distanceTraveled += Vector3.Distance (last, gameObject.transform.position);
		if (timed ? Time.timeSinceLevelLoad - startTime > range : distanceTraveled >= range * MapGenerator.step) {
			if (explodes) {
                explode();
			}
			else
				Destroy (gameObject);
		}
        last = gameObject.transform.position;
	}

    private void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }

    void OnTriggerEnter(Collider col)
	{
		float distance = Vector3.Distance (last, gameObject.transform.position);

        Unit hitUnit = col.gameObject.GetComponent<Unit> ();
        TileManager tm = col.gameObject.GetComponentInParent<TileManager>();

        int damage;
        if (damageFalloff)
            damage = gameObject.tag == "Sniper" ? (int)(maxDamage * (1 + (distance / (range * MapGenerator.step)))) : (int)(maxDamage * (1 - (distance / (2 * range * MapGenerator.step))));
        else
            damage = maxDamage;


		if (hitUnit != null && col.gameObject != origin.gameObject && !hitUnit.IsDead && tm == null)
        {
            if (explodes)
                explode();
            else
            {
                if (hitEffect != null)
                {
                    Instantiate(hitEffect, transform.position, transform.rotation);

                }
                if (hitUnit.team != origin.team)
                {
                    if (projectileEffect != null)
                        projectileEffect.affect(origin, hitUnit);
                    hitUnit.takeDamage(damage); // multiply by 2 to min at half damage
                    if (hitUnit.IsDead)
                        origin.gainKillXP(hitUnit);
                    else
                        origin.gainDamageXP(hitUnit);
                }
                Destroy(gameObject);
            }
        }
        else if (tm != null && !tm.Destroyed)
        {
            if (explodes)
                explode();
            else
            {
                tm.hit(damage, gameObject);
                if (hitEffect != null)
                {
                    Instantiate(hitEffect, transform.position, transform.rotation);
                }
                Destroy(gameObject);
            }
        }
	}

    private void explode()
    {
        Instantiate(explodeParticle, transform.position, transform.rotation);
		//AudioSource.PlayClipAtPoint(explosion, Camera.main.transform.position);
        Collider[] allColliders = Physics.OverlapSphere(transform.position, explodeRange * MapGenerator.step);
        foreach (Collider c in allColliders)
        {
            int damage;
            if (damageFalloff)
                damage = (int)(maxDamage * (1 - Vector3.Distance(c.gameObject.transform.position, this.gameObject.transform.position) / (2 * explodeRange * MapGenerator.step)));
            else
                damage = maxDamage;

            Unit t = c.gameObject.GetComponent<Unit>();
            if(t != null && c.gameObject != origin.gameObject)
            {
                if(projectileEffect != null)
                    projectileEffect.affect(origin, t);
                if (!t.team.Equals(origin.team))
                {
                    if (t.IsDead)
                        continue;
                    t.takeDamage(damage);
                    if (t.IsDead)
                    {
                        origin.gainKillXP(t);
                    }
                    else
                    {
                        origin.gainDamageXP(t);
                    }
                }
            }

            TileManager tm = c.gameObject.GetComponent<TileManager>();
            if (tm != null && !tm.Destroyed)
                tm.hit(damage, gameObject);
        }
        Destroy(gameObject);
    }

}

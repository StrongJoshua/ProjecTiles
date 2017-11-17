﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medic : MonoBehaviour
{
    public List<Collider> triggerList;
    float currTime;
    float delay;
    Unit parent;
    //public MeshRenderer ringRenderer;
	public ParticleSystem healingRing;
    // Use this for initialization
    void Start()
    {
        currTime = Time.timeSinceLevelLoad;
        delay = 2f;
        parent = GetComponentInParent<Unit>();
       // meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
		

        triggerList.RemoveAll((collider) => collider == null);
        if (Time.timeSinceLevelLoad - currTime > delay)
		{
			Collider[] allColliders = Physics.OverlapSphere (transform.position, 7.5f);
            currTime = Time.timeSinceLevelLoad;
			foreach (Collider c in allColliders)
            {
                Unit unit = c.gameObject.GetComponent<Unit>();
				if (unit != null && unit != parent && parent.team == unit.team )
                {
                    unit.heal(1);
					print ("Healing" + unit.name);
                }
            }
        }

        //ringRenderer.enabled = parent.highlighted;
		if (parent.highlighted)
			healingRing.Play ();
		else
			healingRing.Pause ();
    }

    void OnTriggerEnter(Collider col)
    {
        if (!triggerList.Contains(col))
            triggerList.Add(col);
    }
    void OnTriggerExit(Collider col)
    {
        if (triggerList.Contains(col))
            triggerList.Remove(col);
    }
}

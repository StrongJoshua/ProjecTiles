using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medic : MonoBehaviour
{
    public float healRadius;
	public List<Collider> triggerList;
	float currTime;
	float delay;
	Unit parent;
	//public MeshRenderer ringRenderer;
	public ParticleSystem healingRing;
	public GameObject healthPack;
	// Use this for initialization
	void Start ()
	{
		currTime = Time.timeSinceLevelLoad;
		delay = 2f;
		parent = GetComponentInParent<Unit> ();
        healingRing.Play();
        healingRing.gameObject.SetActive(false);
    }

	// Update is called once per frame
	void Update ()
	{
		triggerList.RemoveAll ((collider) => collider == null);
		if (Time.timeSinceLevelLoad - currTime > delay) {
			Collider[] allColliders = Physics.OverlapSphere (transform.position, healRadius * MapGenerator.step);
			currTime = Time.timeSinceLevelLoad;
			foreach (Collider c in allColliders) {
				Unit unit = c.gameObject.GetComponent<Unit> ();
				if (unit != null && parent.team == unit.team && (c.gameObject.GetComponent<Medic>() == null || c.gameObject.GetComponent<Medic>() == this)) {
					unit.heal (this.GetComponent<Unit> ().Level);
				}
			}
		}
        
		if (parent.highlighted) {
			healingRing.gameObject.SetActive (true);
		} else if (!parent.highlighted) {
            healingRing.gameObject.SetActive (false);
		}
	}

	void OnTriggerEnter (Collider col)
	{
		if (!triggerList.Contains (col))
			triggerList.Add (col);
	}

	void OnTriggerExit (Collider col)
	{
		if (triggerList.Contains (col))
			triggerList.Remove (col);
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (transform.position, healRadius * MapGenerator.step);
	}
}

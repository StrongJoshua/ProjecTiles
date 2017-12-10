using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUI : MonoBehaviour {

    Transform unitTransform;
    Vector3 diff;
    public float offset;
    public float yOffset;

	// Use this for initialization
	void Start () {
        unitTransform = transform.parent;
        diff = transform.parent.position - transform.position;
        transform.parent = null;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = new Vector3(unitTransform.position.x - offset, unitTransform.position.y - yOffset, unitTransform.position.z);
        transform.position = pos + diff;
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
}

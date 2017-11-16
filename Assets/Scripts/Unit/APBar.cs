using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APBar : MonoBehaviour {

    private int offset = 3;
    public float yOffset = 0;
    Vector3 yFreeze;
	// Use this for initialization
	void Start () {
        yFreeze = new Vector3(1f, 0f, 1f);
	}
	
	// Update is called once per frame
	void Update () {
        float oldY = transform.position.y;
        Vector3 vec = (Camera.main.transform.position - transform.parent.position);
        Vector3 newPos = new Vector3(0f, 0f, 0f);
        newPos.y = oldY;
        newPos.x = transform.parent.position.x + Mathf.Cos(Mathf.Atan2(vec.z, vec.x)) * 2;
        newPos.z = transform.parent.position.z + Mathf.Sin(Mathf.Atan2(vec.z, vec.x)) * 2;
        transform.position = newPos;

        transform.rotation = Quaternion.LookRotation(Camera.main.transform.position) * Quaternion.Euler(0, 0, 0);
	}
}

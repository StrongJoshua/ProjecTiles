using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMotion : MonoBehaviour {

    public Animator anim;

	// Use this for initialization
	void Start () {
        anim = this.gameObject.GetComponent<Animator>();
	}

    void OnAnimatorMove()
    {
        if (anim)
        {
            Debug.Log("Moving");
            Vector3 newPosition = transform.position;
            newPosition.z += anim.GetFloat("RunSpeed") * Time.deltaTime;
            transform.position = newPosition;
        }
    }
}

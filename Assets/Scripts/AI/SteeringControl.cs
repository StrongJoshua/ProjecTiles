using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringControl : MonoBehaviour {
    public Vector3 target;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float angle = angleDifference();
        float distance = getDistance();
        anim.SetFloat("turn", angle / 180f);
        anim.SetFloat("foward", Mathf.Min(1, distance / MapGenerator.step));
    }

    private float angleDifference()
    {
        return Vector3.SignedAngle(gameObject.transform.forward, target - gameObject.transform.position, Vector3.up);
    }

    private float getDistance()
    {
        return Vector3.Distance(gameObject.transform.position, target);
    }
}

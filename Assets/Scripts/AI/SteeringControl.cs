using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringControl : MonoBehaviour {
    public Unit target;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float turn = getTurn();
        float distance = getDistance();
        if(distance < .1 * (float) MapGenerator.step)
        {
            anim.SetFloat("turn", 0);
            anim.SetFloat("forward", 0);
        } else
        {
            anim.SetFloat("turn", turn);
            anim.SetFloat("forward", Mathf.Min(1, distance / MapGenerator.step));
        }
    }

    private float getTurn()
    {
        float angle = Vector3.SignedAngle(gameObject.transform.forward, target.transform.position - gameObject.transform.position, Vector3.up);
        if (Mathf.Abs(angle) <= 5)
            return 0;
        return angle / 180f;
    }

    private float getDistance()
    {
        return Vector3.Distance(gameObject.transform.position, target.transform.position);
    }
}

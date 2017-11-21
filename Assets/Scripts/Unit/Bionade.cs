using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bionade : SpecialFire
{
    public int cost;



    public override void fire()
    {
        if (unit.canShoot())
        {
            Projectile projectileInfo = unit.projectileFab.GetComponent<Projectile>();
            projectileInfo.team = unit.team;
            int numToFire = projectileInfo.numToFire;
            float speed = projectileInfo.speed;
            if (unit.anim != null)
            {
                unit.anim.SetTrigger("shoot");
            }
            for (int i = 0; i < numToFire; i++)
            {
                //TODO Animate turn towards aim ring
                transform.rotation = Quaternion.Euler(0, unit.aimRing.transform.rotation.eulerAngles.y + 90, 0);
                GameObject temp = Instantiate(unit.specialFab, transform.position + transform.forward + transform.up, transform.rotation);
                temp.GetComponent<Projectile>().origin = this.gameObject;
                temp.transform.Rotate(new Vector3(90, 0, 0));
                Vector3 aim = this.transform.forward * speed;
                aim.x = aim.x + Random.Range(-unit.gunSpread * (200 - 2.5f * unit.accuracy) / 100f, unit.gunSpread * (200 - 2.5f * unit.accuracy) / 100f);
                //print(aim.ToString());
                temp.GetComponent<Rigidbody>().AddForce(aim);
            }

            unit.costAP(cost);
        }	
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

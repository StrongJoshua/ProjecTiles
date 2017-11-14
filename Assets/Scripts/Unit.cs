using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour {
    new public string name;

    private int health;
    public int AP;
	private int x;
	private int y;

    public int maxHealth;
    public int maxAP;
	public int apChargeRate;
    public int defense;
    public int perception;
	public int accuracy;

    public float healthGrowth;
    public float maxAPGrowth;
	public float apChargeRateGrowth;
    public float defenseGrowth;
    public float perceptionGrowth;
	public float accuracyGrowth;

    public Team team;
	public GameObject projectile;
	public float gunSpread;
	public float projectileSpeed;

    public int X
    {
        get { return x; }
        set
        {
            this.x = value;
            this.transform.position = new Vector3(x * MapGenerator.step, .5f, y * MapGenerator.step);
        }
    }

    public int Y
    {
        get { return y; }
        set
        {
            this.y = value;
            this.transform.position = new Vector3(x * MapGenerator.step, .5f, y * MapGenerator.step);
        }
    }

    public int Health
    {
        get { return health; }
    }

	public enum Team
	{
		player,
		enemy
	}

	// Use this for initialization
	void Awake () {
        health = maxHealth;
	}

    void Start()
    {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void levelUp()
    {
        if (increase(healthGrowth))
            maxHealth++;
        if (increase(maxAPGrowth))
            maxAP++;
        if (increase(apChargeRateGrowth))
            apChargeRate++;
        if (increase(defenseGrowth))
            defense++;
        if (increase(perceptionGrowth))
            perception++;
        if (increase(accuracyGrowth))
            accuracy++;
    }

    private bool increase(float growth)
    {
        return Random.Range(0, 1f) <= growth;
    }

    public void moveTo(int targetX, int targetY)
    {
        this.X = targetX;
        this.Y = targetY;
    }
	public void fire()
	{
		int numToFire = projectile.GetComponent<Projectile> ().type == Projectile.ProjectileType.shotgun ? 5 : 1;
		for (int i = 0; i < numToFire; i++) {
			GameObject temp = Instantiate (projectile);
			temp.transform.rotation = transform.rotation;
			temp.transform.Rotate (new Vector3 (90, 0, 0));
			temp.transform.position = transform.position + transform.forward + transform.up;
			Vector3 aim = this.transform.forward * projectileSpeed;
			aim.x = aim.x + Random.Range (-gunSpread * (200-2.5f*accuracy)/100f, gunSpread * (200-2.5f*accuracy)/100f);
			//print (aim.ToString ());
			temp.GetComponent<Rigidbody> ().AddForce (aim);
		}
	}
}

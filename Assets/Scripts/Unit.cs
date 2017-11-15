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

	public bool selected;

    public float movementSpeed = 1;

    private List<Vector2> path;
    private Vector3 target;

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
		selected = false;
    }
	
	// Update is called once per frame
	void Update () {
		if(selected)
		{
			if (Input.GetKey (KeyCode.D) || Input.GetAxis("Aim") > 0) {
				transform.Rotate (0, 240f * Time.deltaTime * Input.GetAxis("Aim"),0);
			}
			if (Input.GetKey (KeyCode.A) || Input.GetAxis("Aim") < 0) {
				transform.Rotate (0, 240f * Time.deltaTime * Input.GetAxis("Aim"),0);
			}
			if (Input.GetKeyDown (KeyCode.F) || Input.GetButtonDown("Fire1")) {
				fire();
			}
			if (Input.GetKeyDown (KeyCode.Z) || Input.GetButtonDown("Cancel")) {
				selected = false;
				transform.GetChild(2).gameObject.SetActive(false);
			}
		}
		if(path != null)
        {
            this.transform.position += (target - this.transform.position).normalized * MapGenerator.step * movementSpeed * Time.deltaTime;
            if((target - this.transform.position).magnitude <= .1f)
            {
                this.transform.position = target;
                if(path.Count == 0)
                {
                    path = null;
                    target = Vector3.zero;
                } else
                {
                    target = new Vector3(path[0].x * MapGenerator.step, .5f, path[0].y * MapGenerator.step);
                    path.RemoveAt(0);
                }
            }
        }
	}
	public void selectUnit()
		{
			selected = true;
			transform.GetChild(2).gameObject.SetActive(true);
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

    public void moveOnPath(List<Vector2> path)
    {
        target = new Vector3(path[0].x * MapGenerator.step, .5f, path[0].y * MapGenerator.step);
        this.path = path;
        this.path.RemoveAt(0);
    }
}

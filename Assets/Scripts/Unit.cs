using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour {
    public static Vector3 nullVector = new Vector3(-1, -1, -1);

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
    internal GameManager gameManager;

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
        target = nullVector;

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
		if(target != nullVector)
        {
            this.transform.position += (target - this.transform.position).normalized * MapGenerator.step * movementSpeed * Time.deltaTime;
            if((target - this.transform.position).magnitude <= .1f)
            {
                target = nullVector;
                gameManager.movementCallback(this);
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

    public void setTarget(Vector2 mapTarget)
    {
        this.target = new Vector3(mapTarget.x * MapGenerator.step, .5f, mapTarget.y * MapGenerator.step);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
	public static Vector3 nullVector = new Vector3 (-1, -1, -1);

	new public string name;

	public int health;
	public float AP;
	private int x;
	private int y;

	public int maxHealth;
	public float maxAP;
	public int apChargeRate;
	public int perception;
	public int accuracy;

	public float healthGrowth;
	public float maxAPGrowth;
	public float apChargeRateGrowth;
	public float perceptionGrowth;
	public float accuracyGrowth;

	public Team team;

	public GameObject projectileFab;

	public Projectile Projectile {
		get { return projectileFab.GetComponent<Projectile> (); }
	}

	public Image APBar, healthBar;
	public AnimationEventHandler animEvent;
	public Animator anim;

	public float gunSpread;

	public bool highlighted;

	private bool isShooting;

	public bool IsShooting {
		get { return isShooting; }
	}

	public float movementSpeed = 1;

	private bool isMoving;

	public bool IsMoving {
		get { return isMoving; }
	}

	private List<Vector2> path;
	private Vector3 target;
	internal GameManager gameManager;

	//Putting this info here for now
	public int attackCost = 2;
	public float currTime;
	public GameObject aimRing;
	public bool isFlying;

	public int X {
		get { return x; }
		set {
			this.x = value;
			this.transform.position = new Vector3 (x * MapGenerator.step, this.transform.position.y, y * MapGenerator.step);
		}
	}

	public int Y {
		get { return y; }
		set {
			this.y = value;
			this.transform.position = new Vector3 (x * MapGenerator.step, this.transform.position.y, y * MapGenerator.step);
		}
	}

	public Vector2 XY {
		get { return new Vector2 (x, y); }
	}

	private bool isDead;

	public bool IsDead {
		get { return isDead; }
	}

	public enum Team
	{
		player,
		enemy
	}

	// Use this for initialization
	void Awake ()
	{
		health = maxHealth;
		target = nullVector;
		AP = maxAP;
	}

	void Start ()
	{
		isMoving = false;
		highlighted = false;
		isShooting = false;
		currTime = Time.timeSinceLevelLoad;
		aimRing.SetActive (false);
		isDead = false;
	}

	void rechargeAP ()
	{
		if (isMoving || isShooting || AP == maxAP)
			return;
		//Check if AP has changed by a whole number, Ex 2.8->3.1
		float old = AP;
		AP = Mathf.Min (AP + apChargeRate * Time.deltaTime, maxAP);
		if ((int)old < (int)AP) // Update UI if it did
            gameManager.apCallback (this);
	}

	void Update ()
	{
		rechargeAP ();

		if (isShooting) {

			if (Input.GetKey (KeyCode.A)) {
				aimRing.transform.Rotate ( 0, 0, 240f * Time.deltaTime);
			} else if (Input.GetKey (KeyCode.D)) {
				aimRing.transform.Rotate ( 0, 0, -240f * Time.deltaTime);
			} else if (Mathf.Abs( Input.GetAxis ("Vertical") ) > 0.1 || Mathf.Abs( Input.GetAxis ("Horizontal") ) > 0.1){
				float yRot = 0;
				if (Input.GetAxis ("Vertical") < 0)
					yRot += 180 - Input.GetAxis ("Horizontal") * 90;
				else
					yRot += Input.GetAxis ("Horizontal") * 90;
				aimRing.transform.rotation = Quaternion.Euler (90, yRot - 90, 0);
			}
		}
		if (anim != null) {
			anim.SetBool ("isMoving", isMoving);
		}
		if (target != nullVector) {
			Vector3 targetPoint = transform.position - new Vector3 (target.x, transform.position.y, target.z);
			Quaternion targetRotation = Quaternion.LookRotation (-targetPoint, Vector3.up);
			transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * 2.0f);

			this.transform.position += (target - this.transform.position).normalized * MapGenerator.step * movementSpeed * Time.deltaTime;
			if ((target - this.transform.position).magnitude <= .1f) {
				this.X = (int)(target.x / MapGenerator.step);
				this.Y = (int)(target.z / MapGenerator.step);
				target = nullVector;
				finishMovement ();
			}
		}
		Vector3 scale = APBar.rectTransform.localScale;
		scale.x = AP / maxAP;
		APBar.rectTransform.localScale = scale;
	}

	public void Die ()
	{
		isDead = true;
		gameManager.deathCallback (this);
		if (anim != null) {
			anim.SetTrigger ("die");
		}
		if (animEvent != null) {
			animEvent.callback = (gameObject) => {
				Destroy (this.gameObject);
			};
		} else {
			Destroy (this.gameObject);
		}
	}

	public void stopAim ()
	{
		isShooting = false;
		//Aim Ring has to be first child
		//transform.GetChild(0).gameObject.SetActive(false);
		aimRing.SetActive (false);
	}

	public void aim ()
	{
		isShooting = true;
		transform.GetChild (0).gameObject.SetActive (true);
		aimRing.SetActive (true);
	}

	private void levelUp ()
	{
		if (increase (healthGrowth))
			maxHealth++;
		if (increase (maxAPGrowth))
			maxAP++;
		if (increase (apChargeRateGrowth))
			apChargeRate++;
		if (increase (perceptionGrowth))
			perception++;
		if (increase (accuracyGrowth))
			accuracy++;
	}

	private bool increase (float growth)
	{
		return Random.Range (0, 1f) <= growth;
	}

	public void moveTo (int targetX, int targetY)
	{
		this.X = targetX;
		this.Y = targetY;

	}

	public void fire ()
	{
		if (canShoot ()) {
			Projectile projectileInfo = projectileFab.GetComponent<Projectile> (); 
			projectileInfo.team = team;
			int numToFire = projectileInfo.numToFire;
			float speed = projectileInfo.speed;
			if (anim != null) {
				anim.SetTrigger ("shoot");
			}
			for (int i = 0; i < numToFire; i++) {
				//TODO Animate turn towards aim ring
				transform.rotation = Quaternion.Euler (0, aimRing.transform.rotation.eulerAngles.y + 90, 0);
				GameObject temp = Instantiate (projectileFab, transform.position + transform.forward + transform.up, transform.rotation);
				temp.transform.Rotate (new Vector3 (90, 0, 0));
				Vector3 aim = this.transform.forward * speed;
				aim.x = aim.x + Random.Range (-gunSpread * (200 - 2.5f * accuracy) / 100f, gunSpread * (200 - 2.5f * accuracy) / 100f);
				//print(aim.ToString());
				temp.GetComponent<Rigidbody> ().AddForce (aim);
			}

			costAP (attackCost);
		}
	}

	public void takeDamage (int incomingDamage)
	{
		if (IsDead)
			return;
		//For now just deincrement health, we can consider armor and stuff later
		health -= incomingDamage;
		Vector3 scale = healthBar.rectTransform.localScale;
		scale.x = (float)health / maxHealth;
		if (health <= 0) {
			scale.x = 0;
			health = 0;
			Die ();
		}
		healthBar.rectTransform.localScale = scale;
		//Debug.Log (incomingDamage);
	}


	public void setTarget (Vector2 mapTarget)
	{
		isMoving = true;
		this.target = new Vector3 (mapTarget.x * MapGenerator.step, this.transform.position.y, mapTarget.y * MapGenerator.step);
	}

	public void finishMovement ()
	{
		isMoving = false;
		gameManager.movementCallback (this);
	}

	public void costAP (int ap)
	{
		this.AP -= ap;
	}

	public void startle ()
	{
		// TODO call startle animation
	}

	public bool canShoot ()
	{
		return AP >= attackCost;
	}

	public void heal (int amount)
	{
		if (health < maxHealth)
			health += amount;
		else
			health = maxHealth;
		Vector3 scale = healthBar.rectTransform.localScale;
		scale.x = (float)health / maxHealth;
		healthBar.rectTransform.localScale = scale;
	}

	public void lookAt (Vector2 tile)
	{
		Vector2 dif = tile - XY;
		this.gameObject.transform.rotation = Quaternion.LookRotation (new Vector3 (dif.x, 0, dif.y) * MapGenerator.step, Vector3.up);
	}
}

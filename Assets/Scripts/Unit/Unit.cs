using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public static Vector3 nullVector = new Vector3(-1, -1, -1);

    new public string name;

    public int health;
    public float AP;
    private int x;
    private int y;
    public string specialName = "Special";

    public int maxHealth;
    public int maxAP;
    public float apChargeRate;
    public int perception;
    public int accuracy;

    private float hiddenMaxHealth, hiddenMaxAP, hiddenAPChargeRate, hiddenPerception, hiddenAccuracy;

    public float healthGrowth, healthGrowthRate;
    public float maxAPGrowth, maxAPGrowthRate;
    public float apChargeRateGrowth, apChargeRateGrowthRate;
    public float perceptionGrowth, perceptionGrowthRate;
    public float accuracyGrowth, accuracyGrowthRate;

    private int xp;

    public float HealthPercent
    {
        get
        {
            return (float)health / maxHealth;
        }
    }

    public float APPercent
    {
        get
        {
            return AP / maxAP;
        }
    }

    public int XP {
        get { return xp; }
    }

    private int level;

    public int Level {
        get { return level; }
    }

    public Dictionary<string, float> initialStats;
    private Dictionary<string, float> growthRates;
    public Sprite icon;
    public Team team;

    public Player player;

    public GameObject projectileFab;
    public GameObject specialFab;
    public GameObject equippedGun;

    public Projectile Projectile {
        get { return projectileFab.GetComponent<Projectile>(); }
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

    public float movementSpeed;

    private bool isMoving;

    public bool IsMoving {
        get { return isMoving; }
    }

    private List<Vector2> path;
    private Vector3 target;
    internal GameManager gameManager;

    //Putting this info here for now
    public int attackCost;
    public int specialCost;
    public float currTime;
    public GameObject aimRing;
    public bool isFlying;

    public SpecialType specialType;

    private float autoAttackLast;
    private readonly float AutoAttackDelay = 5f;
    public LevelUI levelUI;
    public int X {
        get { return x; }
        set {
            this.x = value;
            this.transform.position = new Vector3(x * MapGenerator.step, this.transform.position.y, y * MapGenerator.step);
        }
    }

    public int Y {
        get { return y; }
        set {
            this.y = value;
            this.transform.position = new Vector3(x * MapGenerator.step, this.transform.position.y, y * MapGenerator.step);
        }
    }

    public Vector2 XY {
        get { return new Vector2(x, y); }
    }

    private bool isDead;

    public bool IsDead {
        get { return isDead; }
    }

    public bool IsMedic { get { return GetComponent<Medic>() != null; } }

    public enum Team
    {
        player,
        enemy
    }

    public enum SpecialType
    {
        sniper,
        bionade,
        bombs,
        drone
    }

    // Use this for initialization
    void Awake()
    {
        resetHealthAP();
        target = nullVector;

    }

    void Start()
    {
        isMoving = false;
        highlighted = false;
        isShooting = false;
        currTime = Time.timeSinceLevelLoad;
        aimRing.SetActive(false);
        isDead = false;
        ParticleSystem.MainModule m = aimRing.GetComponent<ParticleSystem>().main;
        ParticleSystem.MinMaxCurve mmc = m.startLifetime;
        mmc.constant *= Projectile.range;
        m.startLifetime = mmc;
    }

    public void resetHealthAP()
    {
        isDead = false;
        health = maxHealth;
        AP = maxAP;
        updateHealthBar();
        updateAPBar();
    }

    void rechargeAP()
    {
        if (isMoving || isShooting || AP == maxAP)
            return;
        //Check if AP has changed by a whole number, Ex 2.8->3.1
        float old = AP;
        AP = Mathf.Min(AP + apChargeRate * Time.deltaTime, maxAP);
        if ((int)old < (int)AP) // Update UI if it did
            gameManager.uiCallback(this);
    }

    void Update()
    {
        rechargeAP();

        if (isShooting) {

            if (Input.GetKey(KeyCode.LeftArrow)) {
                //aimRing.transform.Rotate (0, 0, 240f * Time.deltaTime);
                transform.Rotate(0, -240f * Time.deltaTime, 0);
            } else if (Input.GetKey(KeyCode.RightArrow)) {
                //aimRing.transform.Rotate (0, 0, -240f * Time.deltaTime);
                transform.Rotate(0, 240f * Time.deltaTime, 0);
            } else if (Mathf.Abs(Input.GetAxis("Vertical")) > 0.1 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1) {
                float yRot = 0;
                if (Input.GetAxis("Vertical") < 0)
                    yRot += 270 - Input.GetAxis("Horizontal") * 90;
                else
                    yRot += 90 + Input.GetAxis("Horizontal") * 90;
                //aimRing.transform.rotation = Quaternion.Euler (90, yRot - 90, 0);
                transform.rotation = Quaternion.Euler(0, yRot - 90, 0);
            }
        }
        if (anim != null) {
            anim.SetBool("isMoving", isMoving);
            anim.SetBool("isShooting", isShooting);
        }
        if (target != nullVector) {
            Vector3 targetPoint = transform.position - new Vector3(target.x, transform.position.y, target.z);
            Quaternion targetRotation = Quaternion.LookRotation(-targetPoint, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2.0f);

            this.transform.position += (target - this.transform.position).normalized * MapGenerator.step * movementSpeed * Time.deltaTime;
            if ((target - this.transform.position).magnitude <= .1f) {
                this.X = (int)(target.x / MapGenerator.step);
                this.Y = (int)(target.z / MapGenerator.step);
                target = nullVector;
                finishMovement();
            }
        }
        autoAttack();
        updateAPBar();
    }

    public void Die()
    {
        isDead = true;
        gameManager.deathCallback(this);

        if (team == Team.enemy)
        {
            GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDManager>().decrementEnemyCount();
        }

		if (name.Equals("Medic")) {
			GetComponent<Medic> ().die ();
		}

        if (player != null) {
            player.killUnit(this);
        }

        if (anim != null) {
            anim.SetTrigger("die");
        }
        if (animEvent != null) {
            animEvent.callback = (gameObject) => {
                if (team == Team.player)
                    this.gameObject.SetActive(false);
                else
                    Destroy(this.gameObject);
            };
        } else {
            if (team == Team.player)
                this.gameObject.SetActive(false);
            else
                Destroy(this.gameObject);
        }
    }

    public void stopAim()
    {
        isShooting = false;
        //Aim Ring has to be first child
        //transform.GetChild(0).gameObject.SetActive(false);
        aimRing.SetActive(false);
    }

    public void aim()
    {
        isShooting = true;
        transform.GetChild(0).gameObject.SetActive(true);
        aimRing.SetActive(true);
    }

    private void levelUp()
    {
        level++;
        if(levelUI != null)
            levelUI.setLevel(level);
        if (increase(healthGrowth))
            hiddenMaxHealth *= 1 + healthGrowthRate;
        if (increase(maxAPGrowth))
            hiddenMaxAP *= 1 + maxAPGrowthRate;
        if (increase(apChargeRateGrowth))
            hiddenAPChargeRate *= 1 + apChargeRateGrowthRate;
        if (increase(perceptionGrowth))
            hiddenPerception *= 1 + perceptionGrowthRate;
        if (increase(accuracyGrowth))
            hiddenAccuracy *= 1 + accuracyGrowthRate;

        maxHealth = (int)hiddenMaxHealth;
        maxAP = (int)hiddenMaxAP;
        apChargeRate = Mathf.Floor(hiddenAPChargeRate * 100) / 100f;
        perception = (int)hiddenPerception;
        accuracy = (int)hiddenAccuracy;
        updateHealthBar();
    }

    private bool increase(float growth)
    {
        return Random.Range(0, 100) < growth;
    }

    public void moveTo(int targetX, int targetY)
    {
        this.X = targetX;
        this.Y = targetY;

    }

    public void setStats(Dictionary<string, float> newStats)
    {
        setHiddenStats(1, newStats);
        
        this.healthGrowth = newStats["maxHPGrowth"];
        this.maxAPGrowth = newStats["maxAPGrowth"];
        this.apChargeRateGrowth = newStats["apChargeRateGrowth"];
        this.accuracyGrowth = newStats["accuracyGrowth"];
        this.perceptionGrowth = newStats["perceptionGrowth"];
    }


    public void setGrowthRates(Dictionary<string, float> growthRates)
    {
        this.healthGrowthRate = growthRates["maxHP"];
        this.maxAPGrowthRate = growthRates["maxAP"];
        this.apChargeRateGrowthRate = growthRates["apChargeRate"];
        this.perceptionGrowthRate = growthRates["perception"];
        this.accuracyGrowthRate = growthRates["accuracy"];
    }

    public void fire(bool fromAuto)
    {
        if (fromAuto || canShoot()) {
            if (anim != null) {
                anim.SetTrigger("shoot");
            }

            if (animEvent != null) {
                animEvent.callback = (gameObject) => {
                    shoot();
                };
            } else {
                shoot();
            }

            if (!fromAuto)
                costAP(attackCost);
        }
    }

    private void shoot()
    {
        Vector3 gunOrigin = transform.position + transform.forward + transform.up;
        if (equippedGun != null) {
            Transform gunOriginObject = equippedGun.transform.Find("gunOrigin");
            if (gunOriginObject != null)
                gunOrigin = gunOriginObject.position;
        }
        Projectile projectileInfo = projectileFab.GetComponent<Projectile>();
        int numToFire = projectileInfo.numToFire;
        float speed = projectileInfo.speed;

        for (int i = 0; i < numToFire; i++) {
            //TODO Animate turn towards aim ring
            transform.rotation = Quaternion.Euler(0, aimRing.transform.rotation.eulerAngles.y + 90, 0);
            aimRing.transform.rotation = Quaternion.Euler(90, transform.rotation.eulerAngles.y - 90, 0);
            GameObject temp = Instantiate(projectileFab, gunOrigin, transform.rotation);
            temp.GetComponent<Projectile>().origin = this;
            temp.transform.Rotate(new Vector3(90, 0, 0));
            Vector3 aim = this.transform.forward * speed;
            aim.x = aim.x + Random.Range(-gunSpread * (200 - 2.5f * accuracy) / 100f, gunSpread * (200 - 2.5f * accuracy) / 100f);
            //print(aim.ToString());
            temp.GetComponent<Rigidbody>().AddForce(aim);
        }
    }

    public void takeDamage(int incomingDamage)
    {
        if (IsDead)
            return;
        //For now just deincrement health, we can consider armor and stuff later
        health -= incomingDamage;
        if (health <= 0) {
            health = 0;
            Die();
        }
        //Debug.Log (incomingDamage);
        updateHealthBar();
    }


    public void updateHealthBar()
    {
        Vector3 scale = healthBar.rectTransform.localScale;
        scale.x = (float)health / maxHealth;
        healthBar.rectTransform.localScale = scale;
    }

    public void updateAPBar()
    {
        Vector3 scale = APBar.rectTransform.localScale;
        scale.x = AP / (float)maxAP;
        APBar.rectTransform.localScale = scale;
    }

    public void setTarget(Vector2 mapTarget)
    {
        isMoving = true;
        this.target = new Vector3(mapTarget.x * MapGenerator.step, this.transform.position.y, mapTarget.y * MapGenerator.step);
    }

    public void finishMovement()
    {
        isMoving = false;
        gameManager.movementCallback(this);
    }

    public void costAP(int ap)
    {
        this.AP -= ap;
        gameManager.uiCallback(this);
    }

    public void startle()
    {
        // TODO call startle animation
    }

    public bool canShoot()
    {
        return AP >= attackCost && !name.Equals("Medic");
    }

    public void heal(int amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        updateHealthBar();
        gameManager.uiCallback(this);
    }

    public void lookAt(Vector2 tile)
    {
        Vector2 dif = tile - XY;
        this.gameObject.transform.rotation = Quaternion.LookRotation(new Vector3(dif.x, 0, dif.y) * MapGenerator.step, Vector3.up);
    }

    public bool canSpecial()
    {
        return AP >= specialCost;
    }

    // Fires special
    public void special(UserControl userControl, Unit target = null)
    {
        if (canSpecial()) {
            if (specialType == SpecialType.drone) {
                transform.rotation = Quaternion.Euler(0, aimRing.transform.rotation.eulerAngles.y + 90, 0);
                aimRing.transform.rotation = Quaternion.Euler(90, transform.rotation.eulerAngles.y - 90, 0);
                GameObject special = Instantiate(specialFab, transform.position, transform.rotation);
				DroneAnimationControl dac = special.GetComponent<DroneAnimationControl>();
				dac.hasUserControl = true;
				dac.origin = this;
				dac.userControl = userControl;

                if(team != Team.player)
                {
					dac.hasUserControl = false;

                    special.GetComponent<SteeringControl>().enabled = true;
                    special.GetComponent<SteeringControl>().target = target;
                }

                if(userControl != null)
                    userControl.phase = UserControl.Phase.free;
                isShooting = false;
                aimRing.SetActive(false);
            } else if (specialType == SpecialType.bionade) {
                anim.SetTrigger("shoot");
                animEvent.callback = (gameObject) => bionadeSpecial();
            } else if (specialType == SpecialType.bombs) {
                StartCoroutine(bombSpecial(userControl));

                if(userControl != null)
                    userControl.returnMapControl();
            } else if (specialType == SpecialType.sniper) {
                anim.SetTrigger("shoot");
                animEvent.callback = (gameObject) => {
                    snipeSpecial();
                };
            }
            costAP(specialCost);
        }
    }

    public void snipeSpecial()
    {
        Vector3 gunOrigin = transform.position + transform.forward + transform.up;
        if (equippedGun != null)
        {
            Transform gunOriginObject = equippedGun.transform.Find("gunOrigin");
            if (gunOriginObject != null)
                gunOrigin = gunOriginObject.position;
        }
        //TODO Animate turn towards aim ring
        transform.rotation = Quaternion.Euler(0, aimRing.transform.rotation.eulerAngles.y + 90, 0);
        aimRing.transform.rotation = Quaternion.Euler(90, transform.rotation.eulerAngles.y - 90, 0);
        GameObject temp = Instantiate(specialFab, gunOrigin, transform.rotation);
        temp.GetComponent<Projectile>().origin = this;
        temp.transform.Rotate(new Vector3(90, 0, 0));
        Vector3 aim = this.transform.forward * temp.GetComponent<Projectile>().speed;
        aim.x = aim.x + Random.Range((200 - 2.5f * accuracy) / 100f, (200 - 2.5f * accuracy) / 100f);
        //print(aim.ToString());
        temp.GetComponent<Rigidbody>().AddForce(aim);

        if (team == Team.player)
            FindObjectOfType<UserControl>().returnMapControl();
    }

    public void bionadeSpecial()
    {
        transform.rotation = Quaternion.Euler(0, aimRing.transform.rotation.eulerAngles.y + 90, 0);
        GameObject temp = Instantiate(specialFab, transform.position + transform.forward + transform.up, transform.rotation);
        temp.GetComponent<Projectile>().origin = this;
        temp.transform.Rotate(new Vector3(90, 0, 0));
        Vector3 aim = this.transform.forward * temp.GetComponent<Projectile>().speed;
        temp.GetComponent<Rigidbody>().AddForce(aim);

        if (team == Team.player)
            FindObjectOfType<UserControl>().returnMapControl();
    }

    public void gainDamageXP(Unit damaged)
    {
        gainXP(50 * damaged.Level);
    }

    public void gainKillXP(Unit killed)
    {
        gainXP(100 * killed.Level);
    }

    private void gainXP(int gained)
    {
        if (this.team != Team.player)
            return;
        xp += Mathf.Min(gained, level * 100);
        if (xp >= level * 100) {
            xp -= level * 100;
            levelUp();
        }
        gameManager.uiCallback(this);
    }

    private void autoAttack()
    {
        if (isDead || isMoving || isShooting)
            return;
        if (Time.timeSinceLevelLoad - autoAttackLast > AutoAttackDelay) {
            Unit[] enemies = gameManager.getOpponents(team);
            Unit target = null;
            float projRange = this.Projectile.range;
            bool onHill = gameManager.getTileTypeFor(this) == Tile.TileType.hill;
            foreach (Unit u in enemies) {
                if (u.IsDead)
                    continue;
                float distance = Vector2.Distance(this.XY, u.XY);

                float percepRange = (projRange - 1) * perception / 100f;
                Tile.TileType t = gameManager.getTileTypeFor(u);
                if ((t == Tile.TileType.forest || t == Tile.TileType.swamp) && !onHill)
                    percepRange *= .8f;

                percepRange += 1;

                if (distance <= percepRange) {
                    if (target == null)
                        target = u;
                    else if (distance < Vector2.Distance(this.XY, target.XY))
                        target = u;
                }
            }
            if (target != null) {
                this.lookAt(target.XY);
                this.fire(true);
            }
            autoAttackLast = Time.timeSinceLevelLoad;
        }
    }

    IEnumerator bombSpecial(UserControl uc)
    {
        if (uc == null)
            uc = GameObject.Find("UserControl").GetComponent<UserControl>();
        isMoving = true;
        Vector3 start = transform.position;
        uc.gameManager.mapGenerator.Tiles[x, y].Impassable = true;
        uc.gameManager.characters[this.x, this.y] = null;
        while (transform.position.y < start.y + 3)
        {
            transform.position = transform.position + new Vector3(0, Time.deltaTime * 2);
            yield return null;
        }
        Vector3 cur = transform.position;
        int bombsDropped = 0;
        while (Vector3.Distance(cur, transform.position) < 5.2 * MapGenerator.step)
        {
            transform.position = transform.position + transform.forward * Time.deltaTime * 3;
            if (Vector3.Distance(cur, transform.position) > (1 + bombsDropped) * MapGenerator.step)
            {
                bombsDropped++;
                GameObject bomb = Instantiate(specialFab, gameObject.transform.position, Quaternion.identity);
                bomb.GetComponent<Rigidbody>().AddForce(new Vector3(0, -1));
                bomb.GetComponent<Projectile>().origin = this;
            }
            yield return null;
        }

        Quaternion q = Quaternion.LookRotation(start - transform.position, Vector3.up);
        while(Quaternion.Angle(q, transform.rotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, q, .1f);
            yield return null;
        }

        while (Vector3.Distance(transform.position, start) > .05)
        {
            Vector3 diff = transform.forward * 4f;
            if (Vector3.Distance(transform.position, start) < diff.magnitude * Time.deltaTime)
                break;
            else
                transform.position = transform.position + diff * Time.deltaTime;
            yield return null;
        }
        transform.position = start;

        q = Quaternion.LookRotation(start - transform.position, Vector3.up);
        while (Quaternion.Angle(q, transform.rotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, q, .1f);
            yield return null;
        }

        uc.gameManager.mapGenerator.Tiles[x, y].Impassable = false;
        uc.gameManager.characters[x, y] = this;
        isMoving = false;
    }

    public void setHiddenStats(int level, Dictionary<string, float> stats)
    {
        this.level = level;

        hiddenMaxHealth = stats["maxHP"];
        maxHealth = (int)hiddenMaxHealth;

        hiddenMaxAP = stats["maxAP"];
        maxAP = (int)hiddenMaxAP;

        hiddenAPChargeRate = stats["apChargeRate"];
        apChargeRate = Mathf.Floor(hiddenAPChargeRate * 100) / 100f;

        hiddenPerception = stats["perception"];
        perception = (int)hiddenPerception;

        hiddenAccuracy = stats["accuracy"];
        accuracy = (int)hiddenAccuracy;
    }

    public Dictionary<string, float> getHiddenStats()
    {
        Dictionary<string, float> stats = new Dictionary<string, float>();
        stats["maxHP"] = hiddenMaxHealth;
        stats["maxAP"] = hiddenMaxAP;
        stats["apChargeRate"] = hiddenAPChargeRate;
        stats["perception"] = hiddenPerception;
        stats["accuracy"] = hiddenAccuracy;
        return stats;
    }

	public void Footstep()
	{
		print ("STEP");
	}

    private void OnDestroy()
    {
        Destroy(APBar.gameObject);
        Destroy(healthBar.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PillarManager : TileManager {

	public Image healthBar;
	public GameObject healthBarContainer;
	public GameObject currPillar;
	public GameObject pillarBase;
	public float fallSpeed;


	public int currHealth, maxHealth, maxDamage;

	private int tileX, tileY;

	GameObject lastHitBy;


	// Use this for initialization
	void Start () {
		//currPillar.transform.localScale = new Vector3(0.333f, 5f, 0.333f);
		//currPillar.transform.localPosition = new Vector3 (0f, 6.7f, 0f);
		currHealth = maxHealth;

		lastHitBy = null;
		//currRock.transform.position = new Vector3(0.002f, 0.75f, -0.002f);

		tileX = gameObject.GetComponentInParent<TileInfo>().x;
		tileY = gameObject.GetComponentInParent<TileInfo>().y;

		DealsDamage = true;
	}

	// Update is called once per frame
	void Update () {

	}

	void remove()
	{
		fall ();
		healthBarContainer.SetActive(false);
		Destroyed = true;
	}

	void fall() {
		Vector3 dirToFall = lastHitBy.transform.up * fallSpeed;
		currPillar.transform.SetParent (null);
		currPillar.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
		currPillar.GetComponent<Rigidbody> ().AddForce (dirToFall);
	}


	public override void hit(int damage, GameObject projectile)
	{
		if (currHealth <= 0) return;
		if (lastHitBy == projectile) return;

		lastHitBy = projectile;

		currHealth -= damage;
		float scale = currHealth / (float)maxHealth;
		if (scale <= 0) scale = 0;
		healthBar.rectTransform.localScale = new Vector3(scale, 1f, 1f);
		if (currHealth <= 0)
		{
			remove();
		}
	}
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrelManager : TileManager {
	public GameObject[] barrels;

	public Image healthBar;
	public GameObject healthBarContainer;
	public GameObject currBarrel;

	public AudioClip explosion;
	public float explodeRange;
	public GameObject explodeParticle;

	public int currHealth, maxHealth, maxDamage;

	private int tileX, tileY;

	GameObject lastHitBy;

	GameObject choose(GameObject[] objects)
	{
		return objects[Random.Range(0, objects.Length)];
	}

	// Use this for initialization
	void Start () {
		GameObject barrel = choose(barrels);
		currBarrel = Instantiate(barrel, this.transform);
		currBarrel.transform.localScale = new Vector3(1.3f, 4.18f, 1.3f);
		currBarrel.transform.localPosition = new Vector3 (0f, 1.407f, 0f);
		currHealth = maxHealth;

		lastHitBy = null;
		//currRock.transform.position = new Vector3(0.002f, 0.75f, -0.002f);

		tileX = gameObject.GetComponentInParent<TileInfo>().x;
		tileY = gameObject.GetComponentInParent<TileInfo>().y;
	}

	// Update is called once per frame
	void Update () {

	}

	void remove()
	{
		explode ();
		Tile tile = GameObject.FindGameObjectWithTag("MapGenerator").GetComponent<MapGenerator>().GetTile(tileX, tileY);
		tile.Impassable = false;
		healthBarContainer.SetActive(false);
        Destroyed = true;
		return;

	}

	private void explode()
	{
		Instantiate(explodeParticle, transform.position, transform.rotation);
		AudioSource.PlayClipAtPoint(explosion, transform.position);
		Collider[] allColliders = Physics.OverlapSphere(transform.position, explodeRange * MapGenerator.step);
		foreach (Collider c in allColliders)
		{
			int damage = (int)(maxDamage * (1 - Vector3.Distance(c.gameObject.transform.position, this.gameObject.transform.position) / (2 * explodeRange * MapGenerator.step)));
			Unit t = c.gameObject.GetComponent<Unit>();
			if(t != null )
			{
				t.takeDamage(damage);
			}

			RockManager rm = c.gameObject.GetComponent<RockManager>();
			if(rm != null)
			{
				rm.hit(damage, this.gameObject);
			}

			BarrelManager bm = c.gameObject.GetComponent<BarrelManager>();
			if(bm != null)
			{
				bm.hit(damage, this.gameObject);
			}
		}
		Destroy(currBarrel);
	}

	public void hit(int damage, GameObject projectile)
	{
        if (currHealth <= 0) return;
		if (lastHitBy != null && lastHitBy == projectile) return;

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

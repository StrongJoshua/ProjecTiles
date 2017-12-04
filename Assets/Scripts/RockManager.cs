using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RockManager : MonoBehaviour {

    public GameObject[] smallRocks, medRocks, bigRocks;

    enum RockType { BIG, MED, SMALL }

    public Image healthBar;
    public GameObject healthBarContainer;
    public GameObject currRock;
    RockType currType;
    public int currHealth, maxHealth;

    public int bigRockHealth;
    public int medRockHealth;
    public int smallRockHealth;

    private int tileX, tileY;

    GameObject lastHitBy;

    GameObject choose(GameObject[] objects)
    {
        return objects[Random.Range(0, objects.Length)];
    }

	// Use this for initialization
	void Start () {
        GameObject bigRock = choose(bigRocks);
        currRock = Instantiate(bigRock, this.transform);
        currRock.transform.localScale = new Vector3(0.5f, 1.575f, 0.499f);
        currType = RockType.BIG;
        currHealth = bigRockHealth;
        maxHealth = bigRockHealth;
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
        Destroy(currRock);
        GameObject nextRock = null;
        if (currType == RockType.BIG)
        {
            nextRock = choose(medRocks);
            currHealth = medRockHealth;
            maxHealth = medRockHealth;
            currType = RockType.MED;
            healthBar.rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (currType == RockType.MED)
        {
            nextRock = choose(smallRocks);
            currHealth = smallRockHealth;
            healthBar.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            maxHealth = smallRockHealth;
            currType = RockType.SMALL;
        }
        else if (currType == RockType.SMALL)
        {
            // reset movement cost
            Tile tile = GameObject.FindGameObjectWithTag("MapGenerator").GetComponent<MapGenerator>().GetTile(tileX, tileY);
            tile.Impassable = false;
            healthBarContainer.SetActive(false);
            return;
        }

        currRock = Instantiate(nextRock, this.transform);
    }

    public void hit(int damage, GameObject projectile)
    {
        if (lastHitBy != null && lastHitBy == projectile) return;

        lastHitBy = projectile;

        currHealth -= damage;
        float scale = (float)currHealth / maxHealth;
        if (scale <= 0) scale = 0;
        healthBar.rectTransform.localScale = new Vector3(scale, 1f, 1f);
        if (currHealth <= 0)
        {
            remove();
        }
    }
}

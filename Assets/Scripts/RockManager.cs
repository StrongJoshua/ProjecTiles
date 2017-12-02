using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockManager : MonoBehaviour {

    public GameObject[] smallRocks, medRocks, bigRocks;

    enum RockType { BIG, MED, SMALL }
    public GameObject currRock;
    RockType currType;
    public int currHealth;

    public int bigRockHealth;
    public int medRockHealth;
    public int smallRockHealth;

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
        lastHitBy = null;
        //currRock.transform.position = new Vector3(0.002f, 0.75f, -0.002f);
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
            currType = RockType.MED;
        }
        else if (currType == RockType.MED)
        {
            nextRock = choose(smallRocks);
            currHealth = smallRockHealth;
            currType = RockType.SMALL;
        }
        else if (currType == RockType.SMALL)
        {
            return;
        }

        currRock = Instantiate(nextRock, this.transform);
    }

    internal void hit(int damage, GameObject projectile)
    {
        if (lastHitBy != null && lastHitBy == projectile) return;

        lastHitBy = projectile;

        currHealth -= damage;
        if (currHealth <= 0)
        {
            remove();
        }
    }
}

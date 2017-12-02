using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockManager : MonoBehaviour {

    public GameObject[] smallRocks, medRocks, bigRocks;

    public GameObject currRock;

    GameObject choose(GameObject[] objects)
    {
        return objects[Random.Range(0, objects.Length)];
    }

	// Use this for initialization
	void Start () {
        Debug.Log("Starting");
        GameObject bigRock = choose(bigRocks);
        currRock = Instantiate(bigRock, this.transform);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    internal void hit(int damage)
    {
        throw new System.NotImplementedException();
    }
}

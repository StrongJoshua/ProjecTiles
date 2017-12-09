using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStep : MonoBehaviour {

    public AudioSource leftSound, rightSound;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void FootstepEvent()
    {
        
        leftSound.Play();

      //  Debug.Log("HEY WHAT UP");
    }

    void FootstepLeftEvent()
    {
        if (!leftSound.isPlaying && !rightSound.isPlaying)
            leftSound.Play();
    }

    void FootstepRightEvent()
    {
        if (!rightSound.isPlaying && !leftSound.isPlaying)
            rightSound.Play();
    }
}

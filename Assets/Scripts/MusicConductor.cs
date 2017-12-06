using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicConductor : MonoBehaviour {
	// Use this for initialization
	void Start () {		
	}
	
	// Update is called once per frame
	void Update () {
		
	
	}

	public void adjustMusicVolume(int volume)
	{
		GetComponent<AudioSource> ().volume = volume/100f;
	}
		
}

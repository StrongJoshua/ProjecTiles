using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicConductor : MonoBehaviour {
	// Use this for initialization
	public AudioClip[] playlist;
	public AudioSource audio;
	void Start () {		
		audio = GetComponent<AudioSource> ();
		audio.volume = PersistentInfo.Instance ().MusicVolume;
	}
	
	// Update is called once per frame
	void Update () {
		if (!audio.isPlaying) {
			audio.clip = playlist [Random.Range (0, playlist.Length)];
			audio.Play ();
		}
	}

	public void adjustMusicVolume(int volume)
	{
		audio.volume = volume/100f;
	}
		
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour {
    public Action<GameObject> callback;
	private AudioSource audio;

    void Awake()
    {
        callback = null;
    }
	void Start()
	{
		audio = GetComponent<AudioSource> ();
		if(audio != null)
			audio.volume = PersistentInfo.Instance ().SFXVolume / 100f;
	}
    void DoDestroy()
    {
        if (callback != null)
        {
            callback(transform.gameObject);
        }
    }

	void DoFire() {
		if (callback != null)
		{
			callback(transform.gameObject);
		}
	}

	void Footstep()
	{
		audio.Play ();
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour {
    public Action<GameObject> callback;
	public AudioClip footstep;

    void Awake()
    {
        callback = null;
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
		AudioSource.PlayClipAtPoint (footstep, transform.position);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectCleaner : MonoBehaviour {
    private ParticleSystem ps;
    private bool started = false;
	private AudioSource audio;

	void Start () {
        ps = GetComponent<ParticleSystem>();
		audio = GetComponent<AudioSource> ();
		audio.volume = PersistentInfo.Instance ().SFXVolume;
        if (ps == null)
            Destroy(this);
	}

	void Update () {
        if (ps.particleCount > 0 && !started)
            started = true;
        if (ps.particleCount == 0 && started)
            Destroy(gameObject);
	}
}

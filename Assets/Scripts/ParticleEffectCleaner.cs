using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectCleaner : MonoBehaviour {
    private ParticleSystem ps;
    private bool started = false;

	void Start () {
        ps = GetComponent<ParticleSystem>();
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

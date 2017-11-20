using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlendColor : MonoBehaviour {
    Gradient gradient;
    Material shader;
    float start;

	void Start () {
        gradient = GetComponent<ParticleSystem>().colorOverLifetime.color.gradient;
        shader = GetComponent<ParticleSystemRenderer>().material;
        start = Time.timeSinceLevelLoad;
    }
	
	void Update () {
        shader.SetColor("_Color", gradient.Evaluate(Time.timeSinceLevelLoad - start));
	}
}

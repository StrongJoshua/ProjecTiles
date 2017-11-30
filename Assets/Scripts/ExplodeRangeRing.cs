using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeRangeRing : MonoBehaviour {
	public Projectile target;
	float radius;
	LineRenderer lineRenderer;
	// Use this for initialization
	void Start () {
		radius = target.explodeRange * MapGenerator.step;
		lineRenderer = gameObject.GetComponent<LineRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		DoRenderer();
	}
	void LateUpdate ()
	{
		transform.position = target.transform.position;
	}

	public void DoRenderer ()
	{
		int numSegments = 128;
		Color c1 = new Color (0.5f, 0.5f, 0.5f, 1);
		lineRenderer.material = new Material (Shader.Find ("Particles/Additive"));
		lineRenderer.SetColors (c1, c1);
		lineRenderer.SetWidth (0.2f, 0.2f);
		lineRenderer.SetVertexCount (numSegments + 1);
		lineRenderer.useWorldSpace = false;

		float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
		float theta = 0f;

		for (int i = 0; i < numSegments + 1; i++) {
			float x = radius * Mathf.Cos (theta);
			float z = radius * Mathf.Sin (theta);
			Vector3 pos = new Vector3 (x, 0, z);
			lineRenderer.SetPosition (i, pos);
			theta += deltaTheta;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class APBar : MonoBehaviour {

    public float yOffset = 0;
    public float xOffset = 0;
    Vector3 yFreeze;
	public Material mat;
    // Use this for initialization
    private Quaternion rot;
    void Start () {
        yFreeze = new Vector3(1f, 0f, 1f);
		Image barHolder = transform.GetChild (0).gameObject.GetComponent<Image> ();
		Image barAmount = transform.GetChild (1).gameObject.GetComponent<Image> ();

		if (mat != null && barHolder != null && barAmount != null) {
			barHolder.material = mat;
			barAmount.material = mat;
		}
        rot = transform.rotation;

	}
	
	// Update is called once per frame
	void Update () {
        return;
        float oldY = transform.position.y;
        Vector3 vec = (Camera.main.transform.position - transform.parent.parent.position);
        Vector3 newPos = new Vector3(0f, 0f, 0f);
        newPos.y = oldY;
        newPos.x = transform.parent.parent.position.x + Mathf.Cos(Mathf.Atan2(vec.z, vec.x)) * 2 + 0f;
        newPos.z = transform.parent.parent.position.z + Mathf.Sin(Mathf.Atan2(vec.z, vec.x)) * 2;
        transform.position = newPos;
        //transform.rotation = rot;
        //transform.rotation = Quaternion.LookRotation(Camera.main.transform.position) * Quaternion.Euler(0, 0, 0);
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }
}

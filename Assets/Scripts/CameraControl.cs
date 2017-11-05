using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
	public Camera cam;
	public int maxZoom;
	public int minZoom;
	public GameObject currentTile;
	public GameObject highlight;
	bool moving;
	float delay;
	float lastTime;
	// Use this for initialization
	void Start () {
		moving = false;
		delay = 0.1f;
		lastTime = Time.timeSinceLevelLoad;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.timeSinceLevelLoad - lastTime > delay) {
			lastTime = Time.timeSinceLevelLoad;
			if (Input.GetKey (KeyCode.UpArrow)) {
				//moving = true;
				highlight.transform.position += Vector3.forward * 3;
//			iTween.MoveTo (cam.gameObject, iTween.Hash("position", cam.gameObject.transform.position + Vector3.forward * 3,"time", 0.2f, "oncomplete","unlock", 
//				"oncompletetarget", this.gameObject, "oncompleteparams", new Hashtable()));
			} else if (Input.GetKey (KeyCode.DownArrow)) {
				//moving = true;
				highlight.transform.position += Vector3.back * 3;
//			iTween.MoveTo (cam.gameObject, iTween.Hash("position", cam.gameObject.transform.position + Vector3.back * 3,"time", 0.2f, "oncomplete","unlock", 
//				"oncompletetarget", this.gameObject, "oncompleteparams", new Hashtable()));
			} else if (Input.GetKey (KeyCode.RightArrow)) {
				//moving = true;
				highlight.transform.position += Vector3.right * 3;
//			iTween.MoveTo (cam.gameObject, iTween.Hash("position", cam.gameObject.transform.position + Vector3.right * 3,"time", 0.2f, "oncomplete","unlock", 
//				"oncompletetarget", this.gameObject, "oncompleteparams", new Hashtable()));
			} else if (Input.GetKey (KeyCode.LeftArrow)) {
				//moving = true;
				highlight.transform.position += Vector3.left * 3;
//			iTween.MoveTo (cam.gameObject, iTween.Hash("position", cam.gameObject.transform.position + Vector3.left * 3,"time", 0.2f, "oncomplete","unlock", 
//				"oncompletetarget", this.gameObject, "oncompleteparams", new Hashtable()));
			}
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0) {
			iTween.MoveTo (cam.gameObject, cam.gameObject.transform.position + transform.TransformDirection( Vector3.forward ) * 6, 0.3f);
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			iTween.MoveTo (cam.gameObject, cam.gameObject.transform.position + transform.TransformDirection( Vector3.back ) * 6, 0.3f);
			
		}

		iTween.MoveUpdate (this.gameObject, highlight.transform.position + new Vector3(0,15f,-45f), 1f);
		
	}

	public void updateTile(GameObject tile)
	{
		currentTile = tile;
		iTween.MoveTo (cam.gameObject, tile.transform.position, 0.5f);
	}
	public void unlock()
	{
		print ("called");
		moving = false;
	}
}

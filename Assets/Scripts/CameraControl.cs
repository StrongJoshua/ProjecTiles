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
	// Use this for initialization
	void Start () {
		moving = false;
	}
	
	// Update is called once per frame
	void Update () {
		print (moving);
		if (Input.GetKeyDown (KeyCode.UpArrow) && !moving) {
			//moving = true;
			highlight.transform.position += Vector3.forward * 3;
//			iTween.MoveTo (cam.gameObject, iTween.Hash("position", cam.gameObject.transform.position + Vector3.forward * 3,"time", 0.2f, "oncomplete","unlock", 
//				"oncompletetarget", this.gameObject, "oncompleteparams", new Hashtable()));
		}
		else if (Input.GetKeyDown (KeyCode.DownArrow) && !moving) {
			//moving = true;
			highlight.transform.position += Vector3.back * 3;
//			iTween.MoveTo (cam.gameObject, iTween.Hash("position", cam.gameObject.transform.position + Vector3.back * 3,"time", 0.2f, "oncomplete","unlock", 
//				"oncompletetarget", this.gameObject, "oncompleteparams", new Hashtable()));
		}
		else if (Input.GetKeyDown (KeyCode.RightArrow) && !moving) {
			//moving = true;
			highlight.transform.position += Vector3.right * 3;
//			iTween.MoveTo (cam.gameObject, iTween.Hash("position", cam.gameObject.transform.position + Vector3.right * 3,"time", 0.2f, "oncomplete","unlock", 
//				"oncompletetarget", this.gameObject, "oncompleteparams", new Hashtable()));
		}
		else if (Input.GetKeyDown (KeyCode.LeftArrow) && !moving) {
			//moving = true;
			highlight.transform.position += Vector3.left * 3;
//			iTween.MoveTo (cam.gameObject, iTween.Hash("position", cam.gameObject.transform.position + Vector3.left * 3,"time", 0.2f, "oncomplete","unlock", 
//				"oncompletetarget", this.gameObject, "oncompleteparams", new Hashtable()));
		}
		else if (Input.GetAxis("Mouse ScrollWheel") > 0) {
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour {
	public Camera cam;
	public int maxZoom;
	public int minZoom;
	public GameObject currentTile;
	public GameObject highlight;
	bool moving;
	float delay;
	float lastTime;
	public int x, y;
	public Text coordinates;
	public MapGenerator map;
	// Use this for initialization
	void Start () {
		moving = false;
		delay = 0.2f;
		lastTime = Time.timeSinceLevelLoad;
		x = 0;
		y = 0;
	}
	
	// Update is called once per frame
	void Update () {
		coordinates.text = map.GetTileType(x,y) + "";
		if (Input.GetKeyUp (KeyCode.UpArrow) || Input.GetKeyUp (KeyCode.DownArrow) || Input.GetKeyUp (KeyCode.RightArrow) || Input.GetKeyUp (KeyCode.LeftArrow)) {
			lastTime = 0;
			delay = 0.2f;
		}
		if (Time.timeSinceLevelLoad - lastTime > delay) {
			if (Input.GetKey (KeyCode.UpArrow) && y < 14) {
				//moving = true;
				highlight.transform.position += Vector3.forward * 3;
				y++;
				lastTime = Time.timeSinceLevelLoad;
				if(delay > 0.1f)
					delay -= 0.04f;
//			iTween.MoveTo (cam.gameObject, iTween.Hash("position", cam.gameObject.transform.position + Vector3.forward * 3,"time", 0.2f, "oncomplete","unlock", 
//				"oncompletetarget", this.gameObject, "oncompleteparams", new Hashtable()));
			} else if (Input.GetKey (KeyCode.DownArrow) && y > 0) {
				//moving = true;
				highlight.transform.position += Vector3.back * 3;
				y--;
				lastTime = Time.timeSinceLevelLoad;
				if(delay > 0.1f)
					delay -= 0.04f;
//			iTween.MoveTo (cam.gameObject, iTween.Hash("position", cam.gameObject.transform.position + Vector3.back * 3,"time", 0.2f, "oncomplete","unlock", 
//				"oncompletetarget", this.gameObject, "oncompleteparams", new Hashtable()));
			} else if (Input.GetKey (KeyCode.RightArrow) && x < 14) {
				//moving = true;
				highlight.transform.position += Vector3.right * 3;
				x++;
				lastTime = Time.timeSinceLevelLoad;
				if(delay > 0.1f)
					delay -= 0.04f;
//			iTween.MoveTo (cam.gameObject, iTween.Hash("position", cam.gameObject.transform.position + Vector3.right * 3,"time", 0.2f, "oncomplete","unlock", 
//				"oncompletetarget", this.gameObject, "oncompleteparams", new Hashtable()));
			} else if (Input.GetKey (KeyCode.LeftArrow) && x > 0) {
				//moving = true;
				highlight.transform.position += Vector3.left * 3;
				x--;
				lastTime = Time.timeSinceLevelLoad;
				if(delay > 0.1f)
					delay -= 0.04f;
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

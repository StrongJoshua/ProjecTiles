using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
	public Camera cam;
	public int maxZoom;
	public int minZoom;
	public GameObject currentTile;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			iTween.MoveTo (cam.gameObject, cam.gameObject.transform.position + Vector3.back * 3, 1f);
		}
		else if (Input.GetKeyDown (KeyCode.DownArrow)) {
			iTween.MoveTo (cam.gameObject, cam.gameObject.transform.position + Vector3.forward * 3, 1f);
		}
		else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			//cam.gameObject.transform.position += transform.TransformDirection( Vector3.right ); 
			iTween.MoveTo (cam.gameObject, cam.gameObject.transform.position + transform.TransformDirection( Vector3.right ) * 3, 1f);
		}
		else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			//cam.gameObject.transform.position += transform.TransformDirection( Vector3.left );
			iTween.MoveTo (cam.gameObject, cam.gameObject.transform.position + transform.TransformDirection( Vector3.left ) * 3, 1f);
		}
		else if (Input.GetAxis("Mouse ScrollWheel") > 0) {
			iTween.MoveTo (cam.gameObject, cam.gameObject.transform.position + transform.TransformDirection( Vector3.forward ) * 6, 0.3f);
		}
		else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			iTween.MoveTo (cam.gameObject, cam.gameObject.transform.position + transform.TransformDirection( Vector3.back ) * 6, 0.3f);
			
		}

		
	}

	public void updateTile(GameObject tile)
	{
		currentTile = tile;
		iTween.MoveTo (cam.gameObject, tile.transform.position, 0.5f);
	}
}

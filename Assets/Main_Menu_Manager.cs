using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main_Menu_Manager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	public void goToScene(string scene) {
		SceneManager.LoadScene (scene);
	}
}

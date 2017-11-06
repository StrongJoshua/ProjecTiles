using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

	public GameObject pauseMenu;
	public bool paused = false;
	// Use this for initialization
	void Start () {
		pauseMenu.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(paused)
			{
				paused = false;
				pauseMenu.SetActive(false);
			}
			else
			{
				paused = true;
				pauseMenu.SetActive(true);
			}
			
		}	
	}

	public void resume()
	{
		paused = false;
		pauseMenu.SetActive(false);
	}

	public void mainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}

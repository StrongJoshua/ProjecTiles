using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour {
    public GameObject mainMenuGroup, creditsGroup, backButton;
    public AudioClip click;
    public EventSystem eventSystem;

    public void showCredits ()
    {
        mainMenuGroup.SetActive(false);
        creditsGroup.SetActive(true);
		backButton.SetActive (true);
		creditsGroup.GetComponent<RectTransform> ().position = new Vector3 (Screen.width/2f, -Screen.height/3f, 0);
        //eventSystem.SetSelectedGameObject(creditsGroup.GetComponentInChildren<Button>().gameObject);
    }

    public void showMainMenu ()
    {
        creditsGroup.SetActive(false);
		backButton.SetActive (false);
        mainMenuGroup.SetActive(true);
		creditsGroup.GetComponent<RectTransform> ().position = new Vector3 (Screen.width/2f, -Screen.height/3f, 0);
        eventSystem.SetSelectedGameObject(mainMenuGroup.GetComponentInChildren<Button>().gameObject);
    }

    public void playClick()
    {
        GetComponent<AudioSource>().PlayOneShot(click);
    }

    public void startGame()
    {
        SceneManager.LoadScene("MapGenTest");
    }

	void Update()
	{
		if (creditsGroup.activeSelf) {
			creditsGroup.GetComponent<RectTransform>().position += new Vector3 (0,Screen.height/10f*Time.deltaTime,0);
		}
		if (creditsGroup.GetComponent<RectTransform> ().position.y > Screen.height * 1.55f) {
			showMainMenu ();
		}
	}
}

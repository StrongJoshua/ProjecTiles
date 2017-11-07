using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour {
    public GameObject mainMenuGroup, creditsGroup, backButton;
    public AudioClip click, hover;
    public EventSystem eventSystem;
    public Animator anim;
    public DontDestroy remover;

	Rect ScreenRect = new Rect(0,0,Screen.width,Screen.height);

    public void showCredits ()
    {
        remover.callback = (GameObject gameObject) =>
        {
            mainMenuGroup.SetActive(false);
        };
        anim.SetTrigger("RemoveMenu");
        creditsGroup.SetActive(true);
		backButton.SetActive (true);
		//negative of half the canvas size (800x600) times the Screen width divided by the canvas width. Screen width/canvas width is the scale factor of the canvas.
		creditsGroup.GetComponent<RectTransform> ().position = new Vector3 (Screen.width/2f, -300 * (Screen.width/800f), 0);

		eventSystem.SetSelectedGameObject(backButton);
    }

    public void showMainMenu ()
    {
        creditsGroup.SetActive(false);
		backButton.SetActive (false);
        mainMenuGroup.SetActive(true);
		creditsGroup.GetComponent<RectTransform> ().position = new Vector3 (Screen.width/2f, -300 * (Screen.width/800f), 0);
        eventSystem.SetSelectedGameObject(mainMenuGroup.GetComponentInChildren<Button>().gameObject);
    }

    public void playClick()
    {
        GetComponent<AudioSource>().PlayOneShot(click);
    }

    public void playHover()
    {
        GetComponent<AudioSource>().PlayOneShot(hover);
    }

    public void startGame()
    {
        // Start animating buttons to fly off
        // Calls an animation event once done to remove elements
        remover.callback = (GameObject gameObject) =>
        {
            Destroy(gameObject);
        };
        anim.SetTrigger("RemoveMenu");

        SceneManager.LoadScene("MapGenTest");
    }

	void Update()
	{
		if (creditsGroup.activeSelf) {
			creditsGroup.GetComponent<RectTransform>().position += new Vector3 (0,Screen.height/10f*Time.deltaTime,0);
		}
		Vector3[] worldCorners = new Vector3[4];
		creditsGroup.GetComponent<RectTransform> ().GetWorldCorners (worldCorners);
		if (!ScreenRect.Contains(worldCorners[0]) && worldCorners[0].y > 0 && creditsGroup.GetComponent<RectTransform> ().position.y > 0) {
			showMainMenu ();
		}
	}
}

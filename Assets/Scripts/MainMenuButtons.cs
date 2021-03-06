﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour
{
	public Canvas canvas;
	public GameObject mainMenuGroup, creditsGroup, backButton, createTeamGroup, settingsGroup, controlsDisplay, musicVolumeSlider, sfxVolumeSlider, tutGroup, tutNext, tutPrev;
	public GameObject[] tutScreens;
	public int tutScreen = 0;
	public AudioClip click, hover;
	public Animator mainAnim, createAnim;
	public AnimationEventHandler mainRemover, createRemover;
	public MusicConductor conductor;

	Rect ScreenRect = new Rect (0, 0, Screen.width, Screen.height);

	void Start ()
	{
		EventSystem.current.SetSelectedGameObject (tutNext);

		creditsGroup.SetActive (false);
		backButton.SetActive (false);
		createTeamGroup.SetActive (false);
		mainMenuGroup.SetActive (false);
		tutGroup.SetActive (true);

		RectTransform rt = createTeamGroup.GetComponent<RectTransform> ();
		rt.position = new Vector3 (Screen.width / 2f, Screen.height / 2f);
	}

	public void showCredits ()
	{
		mainRemover.callback = (GameObject gameObject) => {
			mainMenuGroup.SetActive (false);
		};
		mainAnim.SetTrigger ("RemoveMenu");
		creditsGroup.SetActive (true);
		backButton.SetActive (true);
		//negative of half the canvas size (800x600) times the Screen width divided by the canvas width. Screen width/canvas width is the scale factor of the canvas.
		creditsGroup.GetComponent<RectTransform> ().position = new Vector3 (Screen.width / 2f, -300 * canvas.scaleFactor, 0);
		EventSystem.current.SetSelectedGameObject (backButton);
	}

	public void showMainMenu ()
	{
		creditsGroup.SetActive (false);
		settingsGroup.SetActive (false);
		controlsDisplay.SetActive (false);
		tutGroup.SetActive (false);
		backButton.SetActive (false);
		createTeamGroup.SetActive (false);
		mainMenuGroup.SetActive (true);
        if(EventSystem.current.currentSelectedGameObject != mainMenuGroup.GetComponentInChildren<Button>().gameObject)
		    EventSystem.current.SetSelectedGameObject (mainMenuGroup.GetComponentInChildren<Button>().gameObject);
	}

	public void nextTut ()
	{
		if (tutScreen < tutScreens.Length-1) {
			tutScreens [tutScreen].SetActive (false);
			tutScreens [tutScreen + 1].SetActive (true);
			tutScreen++;
        }

        tutPrev.SetActive(true);

        if (tutScreen == tutScreens.Length - 1)
        {
            tutNext.SetActive(false);
            EventSystem.current.SetSelectedGameObject(tutGroup.GetComponentsInChildren<Button>()[0].gameObject);
        }
    }

	public void prevTut ()
	{
		if (tutScreen > 0) {
			tutScreens [tutScreen].SetActive (false);
			tutScreens [tutScreen - 1].SetActive (true);
			tutScreen--;
        }

        tutNext.SetActive(true);

        if (tutScreen == 0)
        {
            tutPrev.SetActive(false);
            EventSystem.current.SetSelectedGameObject(tutNext);
        }
	}


	public void playClick ()
	{
		GetComponent<AudioSource> ().volume = PersistentInfo.Instance ().SFXVolume;
		GetComponent<AudioSource> ().PlayOneShot (click);
	}

	public void playHover ()
	{
		GetComponent<AudioSource> ().volume = PersistentInfo.Instance ().SFXVolume;
		GetComponent<AudioSource> ().PlayOneShot (hover);
	}

	public void createTeam ()
	{
		mainRemover.callback = (GameObject gameObject) => {
			mainMenuGroup.SetActive (false);
			createTeamGroup.GetComponent<CreateTeamManager> ().switchTo ();
		};
		mainAnim.SetTrigger ("RemoveMenu");

		createTeamGroup.SetActive (true);
		createAnim.SetTrigger ("enter");
	}

	public void startGame ()
	{
		// Start animating buttons to fly off
		// Calls an animation event once done to remove elements
		createRemover.callback = (GameObject gameObject) => {
			Destroy (gameObject);
		};
		createAnim.SetTrigger ("exit");
		SceneManager.LoadScene ("MapGenTest");
	}

	public void quitGame ()
	{
		Application.Quit ();
	}

	public void settings ()
	{
		mainRemover.callback = (GameObject gameObject) => {
			mainMenuGroup.SetActive (false);
		};
		mainAnim.SetTrigger ("RemoveMenu");
		settingsGroup.SetActive (true);
		backButton.SetActive (true);
		musicVolumeSlider.SetActive (false);
		sfxVolumeSlider.SetActive (false);
		EventSystem.current.SetSelectedGameObject (backButton);
	}

	public void controls ()
	{
		controlsDisplay.SetActive (true);
	}

	public void controlsEnter ()
	{
		controlsDisplay.SetActive (true);
	}

	public void controlsExit ()
	{
		controlsDisplay.SetActive (false);
	}

	public void musicVolumeSelect ()
	{
		musicVolumeSlider.SetActive (true);
		EventSystem.current.SetSelectedGameObject (musicVolumeSlider);
		musicVolumeSlider.GetComponent<Slider> ().value = PersistentInfo.Instance ().MusicVolume;
	}

	public void musicVolumeAdjust ()
	{
		PersistentInfo pi = PersistentInfo.Instance ();
		pi.MusicVolume = musicVolumeSlider.GetComponent<Slider> ().value;
		conductor.adjustMusicVolume (pi.MusicVolume);
	}

	public void musicVolumePicked ()
	{
		musicVolumeSlider.SetActive (false);
		EventSystem.current.SetSelectedGameObject (backButton);
	}

	public void sfxVolumeSelect ()
	{
		sfxVolumeSlider.SetActive (true);
		EventSystem.current.SetSelectedGameObject (sfxVolumeSlider);
		sfxVolumeSlider.GetComponent<Slider> ().value = PersistentInfo.Instance ().SFXVolume;
	}

	public void sfxVolumeAdjust ()
	{
		PersistentInfo pi = PersistentInfo.Instance ();
		pi.SFXVolume = sfxVolumeSlider.GetComponent<Slider> ().value;
	}

	public void sfxVolumePicked ()
	{
		sfxVolumeSlider.SetActive (false);
		EventSystem.current.SetSelectedGameObject (backButton);
	}

    bool justCanceled;

	void Update ()
	{
        if (Input.GetAxis("Cancel") > 0 && !justCanceled)
        {
            justCanceled = true;
            if (settingsGroup.activeSelf && musicVolumeSlider.activeSelf)
            {
                musicVolumePicked();
            }
            else if (settingsGroup.activeSelf && sfxVolumeSlider.activeSelf)
            {
                sfxVolumePicked();
            }
            else if (createTeamGroup.GetComponent<CreateTeamManager>().allowBack())
            {
                showMainMenu();
                return;
            }
        }
        else if (Input.GetAxis("Cancel") == 0)
            justCanceled = false;

		if (creditsGroup.activeSelf) {
			creditsGroup.GetComponent<RectTransform> ().position += new Vector3 (0, Screen.height / 10f * Time.deltaTime, 0);
		}
		Vector3[] worldCorners = new Vector3[4];
		creditsGroup.GetComponent<RectTransform> ().GetWorldCorners (worldCorners);
		if (!ScreenRect.Contains (worldCorners [0]) && worldCorners [0].y > 0 && creditsGroup.GetComponent<RectTransform> ().position.y > 0) {
			showMainMenu ();
		}
	}
}

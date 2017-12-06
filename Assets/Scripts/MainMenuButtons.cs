using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour {
    public Canvas canvas;
    public GameObject mainMenuGroup, creditsGroup, backButton, createTeamGroup, settingsGroup, controlsDisplay, volumeSlider;
    public AudioClip click, hover;
    public Animator mainAnim, createAnim;
    public AnimationEventHandler mainRemover, createRemover;

	Rect ScreenRect = new Rect(0,0,Screen.width,Screen.height);

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(mainMenuGroup.GetComponentInChildren<Button>().gameObject);

        creditsGroup.SetActive(false);
        backButton.SetActive(false);
        createTeamGroup.SetActive(false);

        RectTransform rt = createTeamGroup.GetComponent<RectTransform>();
        rt.position = new Vector3(Screen.width / 2f, Screen.height / 2f);
    }

    public void showCredits ()
    {
        mainRemover.callback = (GameObject gameObject) =>
        {
            mainMenuGroup.SetActive(false);
        };
        mainAnim.SetTrigger("RemoveMenu");
        creditsGroup.SetActive(true);
		backButton.SetActive (true);
		//negative of half the canvas size (800x600) times the Screen width divided by the canvas width. Screen width/canvas width is the scale factor of the canvas.
        creditsGroup.GetComponent<RectTransform>().position = new Vector3(Screen.width / 2f, -300 * canvas.scaleFactor, 0);
        EventSystem.current.SetSelectedGameObject(backButton);
    }

    public void showMainMenu ()
    {
        creditsGroup.SetActive(false);
		settingsGroup.SetActive(false);
		controlsDisplay.SetActive(false);
		backButton.SetActive(false);
        createTeamGroup.SetActive(false);
        mainMenuGroup.SetActive(true);
        EventSystem.current.SetSelectedGameObject(mainMenuGroup.GetComponentInChildren<Button>().gameObject);
    }

    public void playClick()
    {
        GetComponent<AudioSource>().PlayOneShot(click);
    }

    public void playHover()
    {
        GetComponent<AudioSource>().PlayOneShot(hover);
    }

    public void createTeam()
    {
        mainRemover.callback = (GameObject gameObject) =>
        {
            mainMenuGroup.SetActive(false);
            createTeamGroup.GetComponent<CreateTeamManager>().switchTo();
        };
        mainAnim.SetTrigger("RemoveMenu");

        createTeamGroup.SetActive(true);
        createAnim.SetTrigger("enter");
    }

    public void startGame()
    {
        // Start animating buttons to fly off
        // Calls an animation event once done to remove elements
        createRemover.callback = (GameObject gameObject) =>
        {
            Destroy(gameObject);
        };
        createAnim.SetTrigger("exit");
        SceneManager.LoadScene("MapGenTest");
    }

	public void quitGame(){
		Application.Quit();
	}

	public void settings()
	{
		mainRemover.callback = (GameObject gameObject) =>
		{
			mainMenuGroup.SetActive(false);
		};
		mainAnim.SetTrigger("RemoveMenu");
		settingsGroup.SetActive(true);
		backButton.SetActive (true);
		volumeSlider.SetActive (false);
		EventSystem.current.SetSelectedGameObject(backButton);
	}

	public void controls()
	{
		controlsDisplay.SetActive(true);
	}

	public void controlsEnter()
	{
		controlsDisplay.SetActive(true);
	}
	public void controlsExit()
	{
		controlsDisplay.SetActive(false);
	}

	public void volumeAdjust()
	{
		volumeSlider.SetActive (true);
		EventSystem.current.SetSelectedGameObject(volumeSlider);

	}

	public void volumePicked()
	{
		volumeSlider.SetActive (false);
		EventSystem.current.SetSelectedGameObject(backButton);
	}

	void Update()
	{
        if(Input.GetAxis("Cancel") > 0 && createTeamGroup.GetComponent<CreateTeamManager>().allowBack())
        {
            showMainMenu();
            return;
        }

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

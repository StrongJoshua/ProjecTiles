using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour {
    public GameObject mainMenuGroup, creditsGroup;
    public AudioClip click;
    public EventSystem eventSystem;

    public void showCredits ()
    {
        mainMenuGroup.SetActive(false);
        creditsGroup.SetActive(true);
        eventSystem.SetSelectedGameObject(creditsGroup.GetComponentInChildren<Button>().gameObject);
    }

    public void showMainMenu ()
    {
        creditsGroup.SetActive(false);
        mainMenuGroup.SetActive(true);
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
}

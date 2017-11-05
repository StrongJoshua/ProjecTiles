using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour {
    public GameObject mainMenuGroup, creditsGroup;
    public AudioClip click;

    public void showCredits ()
    {
        mainMenuGroup.SetActive(false);
        creditsGroup.SetActive(true);
    }

    public void showMainMenu ()
    {
        creditsGroup.SetActive(false);
        mainMenuGroup.SetActive(true);
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

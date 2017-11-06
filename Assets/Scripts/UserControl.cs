﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserControl : MonoBehaviour
{
    public Camera cam;
    public int maxZoom;
    public int minZoom;
    public GameObject highlight;
    public int x, y;
    public Text coordinates;
    public float lerpSmooth;

    public MapGenerator map;
    public GameManager gameManager;
    public Canvas canvas;
    public GameObject unitInfo;

	public GameObject pauseMenu;
	public bool paused = false;

    private int xDel, yDel;
    private float delay, lastTime;

    private const float defaultDelay = .2f;

    private SelectedHighlight selector;

    void Start()
    {
        delay = defaultDelay;
        lastTime = 0;
        x = 0;
        y = 0;
        xDel = 0;
        yDel = 0;
		selector = cam.gameObject.GetComponent<SelectedHighlight>();
		pauseMenu.SetActive(false);
    }

    void OnPreRender()
    {
		print(x);
        selector.curTileX = x;
        selector.curTileY = y;
    }

    // Update is called once per frame
    void Update()
    {
		if(!paused){
	        xDel = 0;
	        yDel = 0;

	        if (Input.GetKey(KeyCode.UpArrow))
	            yDel += 1;
	        if (Input.GetKey(KeyCode.DownArrow))
	            yDel -= 1;
	        if (Input.GetKey(KeyCode.RightArrow))
	            xDel += 1;
	        if (Input.GetKey(KeyCode.LeftArrow))
	            xDel -= 1;

	        moveHighlight();
	        coordinates.text = map.GetTileType(x, y) + "";
	        showUnitInfo();
	        
	        if (Input.GetAxis("Mouse ScrollWheel") > 0)
	        {
	            iTween.MoveTo(cam.gameObject, cam.gameObject.transform.position + transform.TransformDirection(Vector3.forward) * 6, 0.3f);
	        }
	        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
	        {
	            iTween.MoveTo(cam.gameObject, cam.gameObject.transform.position + transform.TransformDirection(Vector3.back) * 6, 0.3f);
	        }
		}

		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(paused)
			{
				paused = false;
				pauseMenu.SetActive(paused);
			}
			else
			{
				paused = true;
				pauseMenu.SetActive(paused);
			}

		}	
    }

    private void moveHighlight()
    {
        if (Time.timeSinceLevelLoad - lastTime > delay)
        {
            if (xDel == 0 && yDel == 0)
            {
                lastTime = 0;
                delay = defaultDelay;
            } else {
                x += xDel;
                y += yDel;
                x = Mathf.Max(Mathf.Min(x, map.SizeX - 1), 0);
                y = Mathf.Max(Mathf.Min(y, map.SizeY - 1), 0);
                highlight.transform.position = new Vector3(x * MapGenerator.step, 0, y * MapGenerator.step);

                if (delay > .1f)
                    delay -= .04f;
                lastTime = Time.timeSinceLevelLoad;
            }
        }
        cam.gameObject.transform.position = Vector3.Lerp(cam.transform.position, highlight.transform.position + new Vector3(0, 20f, -45f), lerpSmooth);
    }

    private void showUnitInfo()
    {
        Unit unit = gameManager.unitAt(x, y);
        if (unit == null)
            unitInfo.SetActive(false);
        else
        {
            unitInfo.SetActive(true);
            populateInfoWindow(unit);
            unitInfo.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 - unitInfo.GetComponent<RectTransform>().rect.height / 2 * canvas.scaleFactor);
        }
    }

	public void resumeGame()
	{
		paused = false;
		pauseMenu.SetActive(false);
	}

	public void loadMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}

    private void populateInfoWindow(Unit unit)
    {
        foreach(Text text in unitInfo.GetComponentsInChildren<Text>())
        {
            if (text.name == "Name")
                text.text = unit.name;
            else if (text.name == "HP")
                text.text = unit.Health + "/" + unit.maxHealth;
            else if (text.name == "AP")
                text.text = unit.AP + "/" + unit.maxAP;
            else if (text.name == "APCharge")
                text.text = unit.apChargeRate + "";
            else if (text.name == "Def")
                text.text = unit.defense + "";
            else if (text.name == "Percep")
                text.text = unit.perception + "";
            else if (text.name == "Acc")
                text.text = unit.accuracy + "";
        }
    }
}

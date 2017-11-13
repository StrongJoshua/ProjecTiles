using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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
    public GameObject unitMenu;

    public EventSystem eventSystem;
    public GameObject pauseMenu;
    public bool mapControl = true;

    private int xDel, yDel;
    private float delay, lastTime;

    private const float defaultDelay = .2f;

    private SelectedHighlight selector;
    private Phase phase;

    public bool Paused {
        get { return pauseMenu.activeSelf; }
    }

    private enum Phase
    {
        free,
        movement,
        shoot
    }

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

        unitInfo.transform.position = new Vector3(Screen.width / 2, Screen.height / 2 - unitInfo.GetComponent<RectTransform>().rect.height / 2 * canvas.scaleFactor);
        RectTransform rt = unitMenu.GetComponent<RectTransform>();
        unitMenu.transform.position = new Vector3(Screen.width / 2 + rt.rect.width * 3 / 4 * canvas.scaleFactor, Screen.height / 2 + rt.rect.height / 2 * canvas.scaleFactor);

        coordinates.text = map.GetTileType(x, y) + "";
        showUnitInfo(gameManager.unitAt(x, y));
        phase = Phase.free;
    }

    void OnPreRender()
    {
        selector.curTileX = x;
        selector.curTileY = y;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Paused) {
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
 
            Unit unit = gameManager.unitAt(x, y);

            if (xDel != 0 || yDel != 0)
            {
                coordinates.text = map.GetTileType(x, y) + "";
                showUnitInfo(unit);
            }
	        
	        if (Input.GetAxis("Mouse ScrollWheel") > 0)
	        {
	            iTween.MoveTo(cam.gameObject, cam.gameObject.transform.position + transform.TransformDirection(Vector3.forward) * 6, 0.3f);
	        }
	        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
	        {
	            iTween.MoveTo(cam.gameObject, cam.gameObject.transform.position + transform.TransformDirection(Vector3.back) * 6, 0.3f);
	        }

            if(Input.GetButtonDown("Fire1") && mapControl)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.transform.parent.tag == "Tile") {
                        TileInfo info = hit.collider.gameObject.GetComponent<TileInfo>();
                        if (info != null)
                        {
                            moveHighlightAbsolute(info.x, info.y);
                            coordinates.text = map.GetTileType(x, y) + "";
                            showUnitInfo(unit);
                        }
                    }
                  //  Debug.Log ("object that was hit: "+ourObject);
                }
            }
            if (Input.GetButtonDown("Fire2") && mapControl)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.transform.parent.tag == "Tile")
                    {
                        TileInfo info = hit.collider.gameObject.GetComponent<TileInfo>();
                        if (info != null)
                        {
                            // Move selected unit to clicked target
                            gameManager.moveSelectedUnit(x, y, info.x, info.y);
                        }
                    }
                }
            }
            if(Input.GetKeyDown(KeyCode.Z))
            {
                if (phase == Phase.free)
                {
                    openUnitWindow(unit);
                } else if(phase == Phase.movement)
                {

                }
            }
		}

		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(Paused)
			{
                resumeGame();
			}
			else
			{
                pauseGame();
			}

		}

        if (Input.GetKeyDown(KeyCode.X))
        {
            closeAll();
        }
    }

    private void moveHighlightAbsolute(int newX, int newY)
    {
        if (Time.timeSinceLevelLoad - lastTime > delay)
        {
            if (newX == 0 && newY == 0)
            {
                lastTime = 0;
                delay = defaultDelay;
            }
            else
            {
                x = newX;
                y = newY;
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

    private void moveHighlight()
    {
        if (mapControl && Time.timeSinceLevelLoad - lastTime > delay)
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

    private void showUnitInfo(Unit unit)
    {
        if (unit == null)
        {
            unitInfo.SetActive(false);
            if(phase != Phase.movement)
                hideMovement();
        }
        else
        {
            unitInfo.SetActive(true);
            populateInfoWindow(unit);
            if (phase == Phase.free)
            {
                if (unit.team == Unit.Team.player)
                    showMovement(unit, x, y);
                else
                    hideMovement();
            }
        }
    }

	public void resumeGame()
	{
		mapControl = true;
		pauseMenu.SetActive(false);
        Time.timeScale = 1;
	}

    public void pauseGame()
    {
        mapControl = false;
        unitMenu.SetActive(false);
        pauseMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(pauseMenu.GetComponentInChildren<Button>().gameObject);
        Time.timeScale = 0;
    }

	public void loadMainMenu()
	{
        resumeGame();
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
        Color color = unit.team == Unit.Team.enemy ? gameManager.enemyColor : gameManager.playerColor;
        color.a = unitInfo.GetComponent<Image>().color.a;
        unitInfo.GetComponent<Image>().color = color;
    }

    private void showMovement(Unit u, int x, int y)
    {
        GameObject[,] objects = map.highlights;
        bool[,] movement = AStar.movementMatrix(u.AP, map.Tiles, x, y);
        for(int i = 0; i < movement.GetLength(0); i++)
            for(int j = 0; j < movement.GetLength(1); j++)
            {
                if (!movement[i, j])
                    objects[i, j].SetActive(false);
                else
                    objects[i, j].SetActive(true);
            }
    }

    private void hideMovement()
    {
        foreach (GameObject go in map.highlights)
            go.SetActive(false);
    }

    private void openUnitWindow(Unit unit)
    {
        if (unitMenu.activeSelf || unit == null)
            return;
        if (unit.team == Unit.Team.player)
        {
            unitMenu.SetActive(true);
            mapControl = false;
            eventSystem.SetSelectedGameObject(unitMenu.GetComponentInChildren<Button>().gameObject);
        }
    }

    private void closeAll()
    {
        resumeGame();
        unitMenu.SetActive(false);
        phase = Phase.free;
        showUnitInfo(gameManager.unitAt(x, y));
    }

    public void movementPhase()
    {
        mapControl = true;
        phase = Phase.movement;
        unitMenu.SetActive(false);
    }

    public void shootPhase()
    {
        phase = Phase.shoot;
        unitMenu.SetActive(false);
    }
}

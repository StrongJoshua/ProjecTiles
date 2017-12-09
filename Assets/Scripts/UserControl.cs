using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Threading;
using System;

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
	public GameObject gameOverMenu;
	public GameObject victoryMenu;
    public bool mapControl = true;
	public bool droneControl = false;
	public GameObject drone;

    public GameObject movementArrow;

    private int xDel, yDel;
    private float delay, lastTime;

    private const float defaultDelay = .2f;

    private SelectedHighlight selector;
    public Phase phase;

    private Unit selected, prevHighlight;
	private List<Unit> myUnits;
    private GameObject arrow;
    private List<Vector2> path;

    private Queue<Thread> backgroundTasks;
    private bool isShowingMovement;

    private bool didJustShoot;
	public AudioClip click, hover, tileThud;

    private int cycleIndex = 0;
	private AudioSource audio;

    public bool Paused {
        get { return pauseMenu.activeSelf; }
    }

    public enum Phase
    {
        free,
        movement,
        shoot,
        special
    }

    private void Awake()
    {
        backgroundTasks = new Queue<Thread>();
    }

    void Start()
    {
        delay = defaultDelay;
        lastTime = 0;
        x = gameManager.player.units[0].X;
        y = gameManager.player.units[0].Y;
        moveHighlightAbsolute(x, y);
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

        arrow = Instantiate(movementArrow);
        arrow.SetActive(false);
        gameManager.controlDeathCallback = deathCallback;
		audio = GetComponent<AudioSource> ();
		audio.volume = PersistentInfo.Instance ().SFXVolume / 100f;
    }

    void OnPreRender()
    {
        selector.curTileX = x;
        selector.curTileY = y;
    }

    // Update is called once per frame
    void Update()
    {
        if(backgroundTasks.Count > 0)
        {
            Thread t = backgroundTasks.Peek();
            if (t.ThreadState == ThreadState.Unstarted)
                t.Start();
            else if (t.ThreadState == ThreadState.Stopped)
                backgroundTasks.Dequeue();
        }
        if (!Paused) {
            xDel = 0;
            yDel = 0;

			if (Input.GetAxis("Vertical") > 0)
                yDel += 1;
			if (Input.GetAxis("Vertical") < 0)
                yDel -= 1;
			if (Input.GetAxis("Horizontal") > 0)
                xDel += 1;
			if (Input.GetAxis("Horizontal") < 0)
                xDel -= 1;

            bool didMove = moveHighlight();
 
            Unit unit = gameManager.unitAt(x, y);

            if (unit != null && unit.IsMoving)
                unit = null;
            if (prevHighlight != null)
            {
                prevHighlight.highlighted = false;
            }
            if (unit != null)
            {
                unit.highlighted = true;
                cycleIndex = gameManager.player.units.IndexOf(unit);
            }

			if(didMove)
			{
				//GetComponent<AudioSource>().volume = 0.1f;
				GetComponent<AudioSource>().PlayOneShot(tileThud);
			}

            if (didMove || gameManager.HasUpdate)
            {
                coordinates.text = map.GetTileType(x, y) + "";
                showUnitInfo(unit);
                updateUnitMenu();
            }
            if(phase == Phase.movement && didMove)
            {
                updatePath();
            }
	        
	        //if (Input.GetAxis("Mouse ScrollWheel") > 0)
	        //{
	        //    iTween.MoveTo(cam.gameObject, cam.gameObject.transform.position + transform.TransformDirection(Vector3.forward) * 6, 0.3f);
	        //}
	        //else if (Input.GetAxis("Mouse ScrollWheel") < 0)
	        //{
	        //    iTween.MoveTo(cam.gameObject, cam.gameObject.transform.position + transform.TransformDirection(Vector3.back) * 6, 0.3f);
	        //}

			if(Input.GetMouseButtonDown(0) && mapControl)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit))
                {
					
					if (hit.collider.gameObject.transform.parent != null && hit.collider.gameObject.transform.parent.tag == "Tile") {
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
			if (Input.GetMouseButtonDown(1) && mapControl && phase == Phase.movement)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
					if (hit.collider.gameObject.transform.parent != null && hit.collider.gameObject.transform.parent.tag == "Tile")
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
			if(Input.GetAxis("Submit") > 0)
            {
                if (phase == Phase.free && mapControl)
                {
                    openUnitMenu(unit);
                } else if(phase == Phase.movement)
                {
                    // This is slightly broken because when Z is used to confirm the Move option in the unit menu, this if branch actually gets taken
                    confirmMovement();
                } else if(phase == Phase.shoot && !didJustShoot)
                {
                    unit.fire(false);
                    didJustShoot = true;
                    if (!unit.canShoot())
                    {
                        unit.stopAim();
                        returnMapControl();
                    }
                }
				else if(phase == Phase.special && !didJustShoot)
				{
					unit.special (this);
					didJustShoot = true;
				}
            } else
            {
                didJustShoot = false;
                float cycleInput = Input.GetAxisRaw("Cycle");
                if(cycleInput != 0)
                    delay = defaultDelay;
                if (selected == null && cycleInput != 0 && gameManager.playerUnitsAlive() && Time.timeSinceLevelLoad - lastTime > delay)
                {
                    lastTime = Time.timeSinceLevelLoad;
                    Unit cycleUnit = null;
                    while (cycleUnit == null)
                    {
                        if (cycleInput > 0)
                            cycleIndex += 1;
                        else if (cycleInput < 0)
                            cycleIndex -= 1;
                        cycleIndex = (cycleIndex + gameManager.player.unitsAliveCount()) % gameManager.player.unitsAliveCount();
                        cycleUnit = gameManager.player.units[cycleIndex];
                        if (cycleUnit != null && cycleUnit.IsDead)
                            cycleUnit = null;
                    }
                    moveHighlightAbsolute(cycleUnit.X, cycleUnit.Y);
                    showUnitInfo(cycleUnit);
                }
            }
            prevHighlight = unit;
        }

        if (Input.GetButtonDown("Pause"))
		{
			if (Paused) {
				resumeGame ();
			}  else {
				pauseGame ();
			}

		}

		if (Input.GetButtonDown("Cancel"))
        {
			if (!gameOverMenu.activeSelf && !victoryMenu.activeSelf) {
				closeAll ();
			}
        }

    }

	void LateUpdate()
	{
		if(droneControl)
		{
			Vector3 offset = new Vector3(0f, 3f, -4f);
			transform.position = drone.transform.position + offset;
		}
	}

    private void moveHighlightAbsolute(int newX, int newY)
    {
        x = newX;
        y = newY;
        x = Mathf.Max(Mathf.Min(x, map.SizeX - 1), 0);
        y = Mathf.Max(Mathf.Min(y, map.SizeY - 1), 0);
        highlight.transform.position = new Vector3(x * MapGenerator.step, 0, y * MapGenerator.step);
    }

    private bool moveHighlight()
    {
        bool didMove = false;
        if (mapControl && Time.timeSinceLevelLoad - lastTime > delay)
        {
            if (xDel == 0 && yDel == 0)
            {
                lastTime = 0;
                delay = defaultDelay;
            } else {
                int oldX = x, oldY = y;
                x += xDel;
                y += yDel;
                x = Mathf.Max(Mathf.Min(x, map.SizeX - 1), 0);
                y = Mathf.Max(Mathf.Min(y, map.SizeY - 1), 0);
                if(x != oldX || y != oldY)
                {
                    highlight.transform.position = new Vector3(x * MapGenerator.step, 0, y * MapGenerator.step);

                    if (delay > .1f)
                        delay -= .04f;
                    lastTime = Time.timeSinceLevelLoad;
                    didMove = true;
                }
            }
        }
        cam.gameObject.transform.position = Vector3.Lerp(cam.transform.position, highlight.transform.position + new Vector3(0, 22f, -49.5f), lerpSmooth);
        return didMove;
    }

    private void showUnitInfo(Unit unit)
    {
        if (unit == null)
        {
            unitInfo.SetActive(false);
            if(phase != Phase.movement)
                backgroundTasks.Enqueue(new Thread(() => hideMovement()));
        }
        else
        {
            unitInfo.SetActive(true);
            populateInfoWindow(unit);
            if (phase == Phase.free)
            {
                if (unit.team == Unit.Team.player)
                {
                    backgroundTasks.Enqueue(new Thread(() => showMovement(unit, x, y)));
                }
                else
                    backgroundTasks.Enqueue(new Thread(() => hideMovement()));
            }
        }
    }

	public void resumeGame()
	{
		mapControl = true;
		pauseMenu.SetActive(false);
		gameOverMenu.SetActive (false);
		victoryMenu.SetActive (false);
        Time.timeScale = 1;
	}

	public void retryGame() {
		resumeGame ();
		gameManager.retry ();
	}

	public void nextLevel() {
		resumeGame ();
		gameManager.nextLevel ();
	}

	public void quitGame()
	{
		Application.Quit();
	}

    public void pauseGame()
    {
        
        pauseMenu.SetActive(true);
		pause (pauseMenu);
    }

	private void pause(GameObject menu) {
		mapControl = false;
		unitMenu.SetActive(false);
		EventSystem.current.SetSelectedGameObject(menu.GetComponentInChildren<Button>().gameObject);
		Time.timeScale = 0;
	}

	public void gameOver() {
		gameOverMenu.SetActive (true);
		pause (gameOverMenu);
	}

	public void victory() {
		victoryMenu.SetActive (true);
		pause (victoryMenu);
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
            {
                text.text = unit.name;
            }
            else if (text.name == "HP")
                text.text = unit.health + "/" + unit.maxHealth;
            else if (text.name == "AP")
                text.text = (int)unit.AP + "/" + unit.maxAP;
            else if (text.name == "APCharge")
                text.text = unit.apChargeRate + "";
            else if (text.name == "Percep")
                text.text = unit.perception + "";
            else if (text.name == "Acc")
                text.text = unit.accuracy + "";
            else if (text.name == "XP")
                text.text = unit.XP + "/" + (100 * unit.Level);
        }
        Color color = unit.team == Unit.Team.enemy ? gameManager.enemyColor : gameManager.playerColor;
        color.a = unitInfo.GetComponent<Image>().color.a;
        unitInfo.GetComponent<Image>().color = color;
    }

    private void showMovement(Unit u, int x, int y)
    {
        isShowingMovement = true;
        GameObject[,] objects = map.highlights;
		bool[,] movement = AStar.movementMatrix((int)u.AP, map.Tiles, x, y, u.isFlying);
        for(int i = 0; i < movement.GetLength(0); i++)
            for(int j = 0; j < movement.GetLength(1); j++)
            {
                // Need to create new local variables because i and j are at max when these actions are actually executed
                int k = i;
                int l = j;
                if (!movement[k, l])
                    gameManager.CallOnMainThread(() => objects[k, l].SetActive(false));
                else
                    gameManager.CallOnMainThread(() => objects[k, l].SetActive(true));
            }
    }

    private void hideMovement()
    {
        if (!isShowingMovement)
            return;
        isShowingMovement = false;
        foreach (GameObject go in map.highlights)
            gameManager.CallOnMainThread(() => go.SetActive(false));
    }

    private void openUnitMenu(Unit unit)
    {
        if (unitMenu.activeSelf || unit == null)
            return;
        if (unit.team == Unit.Team.player)
        {
            playClick();
            unitMenu.SetActive(true);
            mapControl = false;
            EventSystem.current.SetSelectedGameObject(unitMenu.GetComponentInChildren<Button>().gameObject);
            unitMenu.GetComponentsInChildren<Button>()[1].interactable = unit.canShoot();
            unitMenu.GetComponentsInChildren<Button>()[2].interactable = unit.canSpecial();
            if (unit.canSpecial())
            {
                unitMenu.GetComponentsInChildren<Button>()[2].GetComponentInChildren<Text>().text = unit.specialName;
            }

            selected = unit;
        }
    }

    public void closeAll()
    {
        resumeGame();
        highlight.SetActive(true);
        unitMenu.SetActive(false);
        phase = Phase.free;
        showUnitInfo(gameManager.unitAt(x, y));
        arrow.SetActive(false);
        if (selected != null)
        {
            selected.stopAim();
        }
        selected = null;
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void movementPhase()
    {
        mapControl = true;
        phase = Phase.movement;
        unitMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        path = new List<Vector2>();
        path.Add(new Vector2(selected.X, selected.Y));
    }

    public void shootPhase()
    {
        phase = Phase.shoot;
        unitMenu.SetActive(false);
		gameManager.unitAt(x, y).aim();
        hideMovement();
        didJustShoot = true;
        highlight.SetActive(false);
    }

    private void updatePath()
    {
        if(map.highlights[x, y].activeSelf)
        {
            addToPath(x, y);
            updateMovementArrow(true);
        } else
        {
            path.RemoveRange(1, path.Count - 1);
            updateMovementArrow(false);
        }
    }

    private void updateMovementArrow(bool show)
    {
        if(!show)
        {
            arrow.SetActive(false);
            return;
        }
        arrow.SetActive(true);
        arrow.GetComponent<ArrowBuilder>().setPath(path);
    }

    private void addToPath(int x, int y)
    {
        Vector2 pos = new Vector2(x, y);
        if(path.Contains(pos))
        {
            int index = path.IndexOf(pos);
            path = path.GetRange(0, index+1);
        } else
        {
            path.Add(pos);
            int cost = 0;
            bool connected = true;
            Vector2 last = new Vector2(selected.X, selected.Y);
            foreach(Vector2 v in path.GetRange(1, path.Count - 1))
            {
                if ((int)Mathf.Abs(last.x - v.x) + (int)Mathf.Abs(last.y - v.y) > 1)
                {
                    connected = false;
                    break;
                }

                last = v;
                cost += selected.isFlying ? 1 : map.Tiles[(int)v.x, (int)v.y].MovementCost;
            }

            if(!connected || cost > selected.AP)
            {
                path = AStar.AStarSearch(map.Tiles, new Vector2(selected.X, selected.Y), new Vector2(x, y), selected.isFlying);
            }
        }
    }

    private void confirmMovement()
    {
        if(path.Count <= 1)
        {
            return;
        }
        playClick();
        gameManager.moveUnitOnPath(selected, path.GetRange(1, path.Count - 1));
        selected = null;
        path = null;
        updateMovementArrow(false);
        hideMovement();
        phase = Phase.free;
    }

    private void updateUnitMenu()
    {
        if (!unitMenu.activeSelf)
            return;
        unitMenu.GetComponentsInChildren<Button>()[1].interactable = selected.canShoot();
        unitMenu.GetComponentsInChildren<Button>()[2].interactable = selected.canSpecial();
    }

    internal void deathCallback(Unit unit)
    {
        if (selected == unit)
            closeAll();
    }

	public void playClick()
	{
		GetComponent<AudioSource>().PlayOneShot(click);
	}

	public void playHover()
	{
		GetComponent<AudioSource>().PlayOneShot(hover);
	}

    public void specialPhase()
    {
        phase = Phase.special;
		unitMenu.SetActive(false);
		gameManager.unitAt(x, y).aim();
		hideMovement();
		didJustShoot = true;
        highlight.SetActive(false);
    }

    public void returnMapControl()
    {
        closeAll();
        mapControl = false;
        StartCoroutine(wait(() => mapControl = true));
    }

    IEnumerator wait(Action action)
    {
        float time = .2f;
        while(time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        action();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Threading;

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

    public bool Paused {
        get { return pauseMenu.activeSelf; }
    }

    public enum Phase
    {
        free,
        movement,
        shoot
    }

    private void Awake()
    {
        backgroundTasks = new Queue<Thread>();
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

        arrow = Instantiate(movementArrow);
        arrow.SetActive(false);
        gameManager.controlDeathCallback = deathCallback;
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
	        
	        if (Input.GetAxis("Mouse ScrollWheel") > 0)
	        {
	            iTween.MoveTo(cam.gameObject, cam.gameObject.transform.position + transform.TransformDirection(Vector3.forward) * 6, 0.3f);
	        }
	        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
	        {
	            iTween.MoveTo(cam.gameObject, cam.gameObject.transform.position + transform.TransformDirection(Vector3.back) * 6, 0.3f);
	        }

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
			if(Input.GetKeyDown(KeyCode.Z) || Input.GetButtonDown("Fire1"))
            {
                if (phase == Phase.free)
                {
                    openUnitMenu(unit);
                } else if(phase == Phase.movement)
                {
                    // This is slightly broken because when Z is used to confirm the Move option in the unit menu, this if branch actually gets taken
                    confirmMovement();
                } else if(phase == Phase.shoot && !didJustShoot)
                {
                    unit.fire();
                    didJustShoot = true;
                }
            } else
            {
                didJustShoot = false;
            }
            prevHighlight = unit;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Pause"))
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

		if (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Cancel"))
        {
            closeAll();
        }

    }

    private void moveHighlightAbsolute(int newX, int newY)
    {
        if (Time.timeSinceLevelLoad - lastTime > delay)
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
        cam.gameObject.transform.position = Vector3.Lerp(cam.transform.position, highlight.transform.position + new Vector3(0, 20f, -45f), lerpSmooth);
  
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
                x += xDel;
                y += yDel;
                x = Mathf.Max(Mathf.Min(x, map.SizeX - 1), 0);
                y = Mathf.Max(Mathf.Min(y, map.SizeY - 1), 0);
                highlight.transform.position = new Vector3(x * MapGenerator.step, 0, y * MapGenerator.step);

                if (delay > .1f)
                    delay -= .04f;
                lastTime = Time.timeSinceLevelLoad;
                didMove = true;
            }
        }
        cam.gameObject.transform.position = Vector3.Lerp(cam.transform.position, highlight.transform.position + new Vector3(0, 20f, -45f), lerpSmooth);
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
                text.text = unit.health + "/" + unit.maxHealth;
            else if (text.name == "AP")
                text.text = (int)unit.AP + "/" + unit.maxAP;
            else if (text.name == "APCharge")
                text.text = unit.apChargeRate + "";
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
            unitMenu.SetActive(true);
            mapControl = false;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(unitMenu.GetComponentInChildren<Button>().gameObject);
            unitMenu.GetComponentsInChildren<Button>()[1].interactable = unit.canShoot();
            selected = unit;
        }
    }

    private void closeAll()
    {
        print("Close all");
        resumeGame();
        unitMenu.SetActive(false);
        phase = Phase.free;
        showUnitInfo(gameManager.unitAt(x, y));
        arrow.SetActive(false);
        if (selected != null)
        {
            selected.stopAim();
        }
        selected = null;
    }

    public void movementPhase()
    {
        mapControl = true;
        phase = Phase.movement;
        unitMenu.SetActive(false);
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
}

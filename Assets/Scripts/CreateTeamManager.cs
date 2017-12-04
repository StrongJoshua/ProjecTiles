using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class CreateTeamManager : MonoBehaviour {
    public InputField nameInput;
    public Dropdown unitTypeDropdown;
    public Button beginButton;
    public ScrollRect team;
    public GameObject buttonPrefab;
    public int unitCount;
    private GameObject[] buttons;
    public GameObject[] unitPrefabs;
    private int[] unitTypes;
    private int selected;

    private float backDelay;
    private GameObject lastGameObject;
    private bool switchToInput;

	void Start () {
        team.verticalScrollbar.value = 1;
        buttons = new GameObject[unitCount];
        unitTypes = new int[unitCount];

		for(int i = 0; i < unitCount; i++)
        {
            GameObject buttonGO = Instantiate(buttonPrefab, team.content.transform);
            buttonGO.GetComponentInChildren<Text>().text = "Unit " + (i + 1);
            RectTransform rt = buttonGO.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(rt.rect.width / 2f, -rt.rect.height / 2 - rt.rect.height * i - 5 * i);

            Button b = buttonGO.GetComponent<Button>();
            int index = i;
            b.onClick.AddListener(() => setInfo(index));
            Navigation buttonNav = b.navigation;
            buttonNav.mode = Navigation.Mode.Explicit;
            if (i > 0)
                buttonNav.selectOnUp = buttons[i - 1].GetComponent<Button>();
            b.navigation = buttonNav;

            buttons[i] = buttonGO;
        }

        Navigation nav = buttons[0].GetComponent<Button>().navigation;
        nav.selectOnUp = beginButton;
        buttons[0].GetComponent<Button>().navigation = nav;

        for(int i = 0; i < buttons.Length - 1; i++)
        {
            nav = buttons[i].GetComponent<Button>().navigation;
            nav.selectOnDown = buttons[i + 1].GetComponent<Button>();
            buttons[i].GetComponent<Button>().navigation = nav;
        }

        nav = buttons[buttons.Length - 1].GetComponent<Button>().navigation;
        nav.selectOnDown = beginButton;
        buttons[buttons.Length - 1].GetComponent<Button>().navigation = nav;

        nav = beginButton.navigation;
        nav.selectOnDown = buttons[0].GetComponent<Button>();
        nav.selectOnUp = buttons[buttons.Length - 1].GetComponent<Button>();
        beginButton.navigation = nav;

        setInfo(0);
	}

    public void switchTo()
    {
        EventSystem.current.SetSelectedGameObject(buttons[selected]);
    }

    public void confirmName()
    {
        buttons[selected].GetComponentInChildren<Text>().text = nameInput.text;
    }

    public void setInfo(int index)
    {
        selected = index;
        nameInput.text = buttons[selected].GetComponentInChildren<Text>().text;
        unitTypeDropdown.value = unitTypes[selected];
        EventSystem.current.SetSelectedGameObject(nameInput.gameObject);
    }

    public bool allowBack()
    {
        if (backDelay > 0)
            return false;
        return EventSystem.current.currentSelectedGameObject != nameInput.gameObject && EventSystem.current.currentSelectedGameObject != unitTypeDropdown;
    }

    public void unitTypeChanged()
    {
        unitTypes[selected] = unitTypeDropdown.value;
    }

    public Player generatePlayer(Color playerColor)
    {
        XMLParser xml = new XMLParser();
        Unit[] playerUnits = new Unit[unitCount];
        for(int i = 0; i < unitCount; i++)
        {
            Unit unit = GameManager.createUnit(unitPrefabs[unitTypes[i]], xml.getBaseStats(unitPrefabs[unitTypes[i]].name), null, playerColor, Unit.Team.player, xml);
            unit.name = buttons[i].GetComponentInChildren<Text>().text;
            playerUnits[i] = unit;
        }

        return new Player(playerUnits);
    }

    private void Update()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;
        if (current != lastGameObject && current == nameInput.gameObject)
            switchToInput = true;

        if(switchToInput && nameInput.isFocused)
        {
            switchToInput = false;
            nameInput.DeactivateInputField();
        }

        if (Input.GetAxis("SubmitInput") > 0)
            nameInput.DeactivateInputField();
        if (Input.GetAxis("Cancel") > 0 && !allowBack())
        {
            if (nameInput.isFocused)
            {
                if (Input.GetKey(KeyCode.X))
                    return;
                nameInput.DeactivateInputField();
            }
            else
            {
                backDelay = 1f;
                switchTo();
            }
        } else if (Input.GetAxis("Cancel") == 0)
            backDelay = 0;

        lastGameObject = current;
    }
}

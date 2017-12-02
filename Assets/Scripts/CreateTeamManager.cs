using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class CreateTeamManager : MonoBehaviour {
    public InputField nameInput;
    public Dropdown unitTypeDropdown;
    public ScrollRect team;
    public GameObject buttonPrefab;
    public int unitCount;
    private GameObject[] buttons;
    public GameObject[] unitPrefabs;
    private int[] unitTypes;
    private int selected;

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
            buttonNav.selectOnRight = nameInput;
            b.navigation = buttonNav;

            buttons[i] = buttonGO;
        }

        Navigation nav = buttons[0].GetComponent<Button>().navigation;
        nav.selectOnUp = buttons[buttons.Length - 1].GetComponent<Button>();

        for(int i = 0; i < buttons.Length; i++)
        {
            nav = buttons[i].GetComponent<Button>().navigation;
            nav.selectOnDown = buttons[(i + 1) % buttons.Length].GetComponent<Button>();
            buttons[i].GetComponent<Button>().navigation = nav;
        }

        setInfo(0);
	}

    public void switchTo()
    {
        EventSystem.current.SetSelectedGameObject(buttons[0]);
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

        Navigation nav = nameInput.navigation;
        nav.selectOnLeft = buttons[selected].GetComponent<Button>();
        nameInput.navigation = nav;

        nav = unitTypeDropdown.navigation;
        nav.selectOnLeft = buttons[selected].GetComponent<Button>();
        unitTypeDropdown.navigation = nav;
    }

    public bool allowBack()
    {
        return EventSystem.current.currentSelectedGameObject != nameInput.gameObject || !nameInput.isFocused;
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
        if (Input.GetAxis("SubmitInput") > 0)
            nameInput.DeactivateInputField();
    }
}

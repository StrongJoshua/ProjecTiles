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
    public GameObject[] units;
    private int[] unitTypes;
    private int selected;

	void Start () {
        team.verticalScrollbar.value = 1;
        buttons = new GameObject[unitCount];
        unitTypes = new int[unitCount];

		for(int i = 0; i < unitCount; i++)
        {
            GameObject button = Instantiate(buttonPrefab, team.content.transform);
            button.GetComponentInChildren<Text>().text = "Unit " + (i + 1);
            RectTransform rt = button.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(rt.rect.width / 2f, -rt.rect.height / 2 - rt.rect.height * i - 5 * i);

            int index = i;
            button.GetComponent<Button>().onClick.AddListener(() => setInfo(index));

            buttons[i] = button;
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
            Unit newUnit = GameManager.createUnit(units[unitTypes[i]], xml.getBaseStats(units[unitTypes[i]].name), null, playerColor, Unit.Team.player, xml);
            playerUnits[i] = newUnit;
            playerUnits[i].name = buttons[i].GetComponentInChildren<Text>().text;
        }

        return new Player(playerUnits);
    }
}

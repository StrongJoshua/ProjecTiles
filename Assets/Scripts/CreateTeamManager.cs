using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateTeamManager : MonoBehaviour {
    public InputField nameInput;
    public Dropdown unitTypeDropdown;
    public ScrollRect team;
    public int unitCount;

	void Start () {
        team.verticalScrollbar.value = 1;
		for(int i = 0; i < unitCount; i++)
        {
        }
	}

    public void confirmName()
    {
        string name = nameInput.text;
    }
}

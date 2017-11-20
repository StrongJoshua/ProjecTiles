using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractableButton : MonoBehaviour {
    public Button onUp, onDown;
    EventSystem eventSystem;
    Button button, lastSelected;

	// Use this for initialization
	void Start () {
        eventSystem = EventSystem.current;
        button = this.GetComponentInChildren<Button>();
	}
	
	// Update is called once per frame
	void Update () {
        if (eventSystem.currentSelectedGameObject == button.gameObject && !button.IsInteractable())
        {
            if (lastSelected == onUp)
                eventSystem.SetSelectedGameObject(onDown.gameObject);
            else
                eventSystem.SetSelectedGameObject(onUp.gameObject);
        }
        lastSelected = eventSystem.currentSelectedGameObject.GetComponent<Button>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractableButton : MonoBehaviour {
    public EventSystem eventSystem;
    Button button;

	// Use this for initialization
	void Start () {
        button = this.GetComponentInChildren<Button>();
	}
	
	// Update is called once per frame
	void Update () {
        if (eventSystem.currentSelectedGameObject == button.gameObject && !button.IsInteractable())
            eventSystem.SetSelectedGameObject(button.FindSelectableOnUp().gameObject);
	}
}

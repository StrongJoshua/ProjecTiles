using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Image image;
    public Sprite idle, hover, click;

	// Use this for initialization
	void Start () {
        image = GetComponent<Image>();
        image.sprite = idle;
	}
	
    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = hover;
        //myText.text = "Hovering";
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = idle;
        //myText.text = "Not Hovering";
    }
}

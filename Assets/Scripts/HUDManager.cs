using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {


    private Player player;
    public GameObject unitImagesContainer;
    public Image unitImagePrefab;

    private List<Image> unitImages;

    // called after units are made by gamemanager
    public void initialize(Player player)
    {
        this.player = player;

        // add units to hud
        drawUnits();
    }

    private void drawUnits()
    {
        unitImages = new List<Image>();
        int count = 0;
        foreach (Unit u in player.units)
        {
            Image image = Instantiate(unitImagePrefab);
            image.rectTransform.SetParent(unitImagesContainer.transform);
            unitImages.Add(image);

            Vector3 pos = new Vector3(0f, 0f, 0f);
            pos.x = count++ * (10 + 20);
            pos.y = 10;
            image.rectTransform.anchoredPosition = pos;
        }
    }

    public void removeUnit(Unit unit)
    {
        foreach (Image image in unitImages)
        {
            Destroy(image);
        }
        // redraw units
        drawUnits();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

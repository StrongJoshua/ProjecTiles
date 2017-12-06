using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {


    private Player player;
    public GameObject unitImagesContainer;
    public GameObject unitImagePrefab;
    public Text enemyCountText;
    public Dictionary<Unit, GameObject> unitImageMap;
    public Dictionary<GameObject, Unit> imageUnitMap;

    int enemyCount;

    private List<GameObject> unitImages;

    // called after units are made by gamemanager
    public void initialize(Player player)
    {
        this.player = player;
        unitImageMap = new Dictionary<Unit, GameObject>();
        imageUnitMap = new Dictionary<GameObject, Unit>();
        // add units to hud
        drawUnits();
    }

    public void Update()
    {
        foreach (Unit u in player.units)
        {
            if (unitImageMap.ContainsKey(u))
            {
                GameObject image = unitImageMap[u];
                UnitImage imageInfo = image.GetComponent<UnitImage>();
                imageInfo.updateHealth(u.HealthPercent);
                imageInfo.updateAP(u.APPercent);
            }
        }
    }

    private void drawUnits()
    {
        unitImages = new List<GameObject>();
        int count = 0;
        foreach (Unit u in player.units)
        {
            if (u.IsDead)
                continue;
            GameObject image = Instantiate(unitImagePrefab);
            image.transform.SetParent(unitImagesContainer.transform);
            unitImages.Add(image);
            unitImageMap.Add(u, image);
            imageUnitMap.Add(image, u);

            Vector3 pos = new Vector3(0f, 0f, 0f);
            pos.x = count++ * (30 + 20) + 40;
            pos.y = 30;
            image.transform.position = pos;
        }
    }

    public void setEnemyCount(int count)
    {
        enemyCount = count;
        enemyCountText.text = "" + enemyCount;
    }

    public void decrementEnemyCount()
    {
        enemyCountText.text = "" + --enemyCount;
    }

    public void removeUnit(Unit unit)
    {
        foreach (GameObject image in unitImages)
        {
            Destroy(image);
            unitImageMap.Remove(imageUnitMap[image]);
        }
        // redraw units
        drawUnits();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalUnitInfo : MonoBehaviour {
    public GameObject infoPrefab;
    public GameObject parent;

    public void setupInfo(List<Unit> units)
    {
        for (int i = 0; i < units.Count; i++)
        {
            GameObject g = Instantiate(infoPrefab, parent.transform);
            Vector3 pos = new Vector3(0f, -i * 40f, 0f);
            g.GetComponent<RectTransform>().anchoredPosition = pos;
            g.GetComponentInChildren<Text>().text = "Level " + units[i].Level;
        }
    }
}

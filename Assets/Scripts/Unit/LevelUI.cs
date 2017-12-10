using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour {

    public Text text;

    public void setLevel(int level)
    {
        text.text = "" + level;
    }
}



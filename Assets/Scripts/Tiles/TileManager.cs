using System;
using UnityEngine;

public class TileManager : MonoBehaviour {
    public Action tileChangedCallback;
    private bool destroyed;

    public bool Destroyed
    {
        get { return destroyed; }
        set
        {
            destroyed = value;
            if(tileChangedCallback != null)
                tileChangedCallback();
        }
    }
}

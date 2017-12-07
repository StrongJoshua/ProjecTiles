using System;
using UnityEngine;

public abstract class TileManager : MonoBehaviour {
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

    private bool dealsDamage;
    public bool DealsDamage
    {
        get { return dealsDamage; }
        protected set { dealsDamage = value; }
    }

    public abstract void hit(int damage, GameObject projectile);
}

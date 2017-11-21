using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base class for unit-specific fire modes, inherit and define behaviors as necessary (see Snipe.cs and Bionade.cs)
public class SpecialFire : MonoBehaviour {

    public Unit unit;
    public virtual void startAim()
    {

    }
    public virtual void stopAim()
    {

    }
    public virtual void fire()
    {
        Debug.Log("parent fire");
    }
}

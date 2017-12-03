using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileEffect : MonoBehaviour {
    public abstract void affect(Unit from, Unit to);
}

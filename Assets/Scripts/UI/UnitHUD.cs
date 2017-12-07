using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitHUD : MonoBehaviour {

    public Image healthBar, apBar;
    public Unit unit;

	public void update()
    {
        Vector3 scale = healthBar.rectTransform.localScale;
        scale.x = unit.HealthPercent;
        healthBar.rectTransform.localScale = scale;

        Vector3 apScale = apBar.rectTransform.localScale;
        apScale.x = unit.APPercent;
        apBar.rectTransform.localScale = apScale;
    }
}

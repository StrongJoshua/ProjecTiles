using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitImage : MonoBehaviour {

    public Image health, AP, icon;

    public void setIcon(Sprite icn)
    {
        icon.sprite = icn;
    }

	public void updateHealth(float percent)
    {
        Vector3 scale = health.rectTransform.localScale;
        scale.x = percent;
        health.rectTransform.localScale = scale;
    }

    public void updateAP(float percent)
    {
        Vector3 scale = AP.rectTransform.localScale;
        scale.x = percent;
        AP.rectTransform.localScale = scale;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationUtils : MonoBehaviour {

    public static void setColor(GameObject obj, Color col)
    {
        SkinnedMeshRenderer[] renderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();

        if (renderers.Length > 0)
        {
            foreach(SkinnedMeshRenderer smr in renderers)
                smr.material.color = col;
        }
    }
}

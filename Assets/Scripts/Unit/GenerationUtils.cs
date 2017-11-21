using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationUtils : MonoBehaviour {

    public static void setColor(GameObject obj, Color col)
    {
        SkinnedMeshRenderer[] renderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
        MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();

        foreach (SkinnedMeshRenderer smr in renderers)
            smr.material.color = Color.Lerp(smr.material.color, col, .5f);
        foreach (MeshRenderer mr in meshes)
            foreach (Material m in mr.materials)
                m.color = Color.Lerp(m.color, col, .5f);
    }
}

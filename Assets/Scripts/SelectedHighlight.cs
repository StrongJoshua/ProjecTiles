using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedHighlight : MonoBehaviour {
    public Color selectColor = new Color(108f / 255f, 221f / 255f, 0, 255f / 255f);
    public int tileStartX, tileStartZ;
    public int stepSize;
    public int curTileX, curTileY;

    private float startX, startZ;

    void Start()
    {
        startX = (float)tileStartX - (float)stepSize / 2;
        startZ = (float)tileStartZ - (float)stepSize / 2;
    }

    static Material selectMaterial;
    static void CreateSelectMaterial()
    {
        if (!selectMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            selectMaterial = new Material(shader);
            selectMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            selectMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            selectMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            selectMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            selectMaterial.SetInt("_ZWrite", 1);
            selectMaterial.SetInt("_ZTest", 0);
        }
    }

    void OnPostRender()
    {
        CreateSelectMaterial();
        selectMaterial.SetPass(0);

        float x = startX + stepSize * curTileX;
        float y = startZ + stepSize * curTileY;
        float thickness = .2f;

        drawHorizontalLine(x, .5f, y, stepSize, thickness);
        drawHorizontalLine(x, .5f, y + stepSize, stepSize, thickness);
        drawVerticalLine(x, .5f, y, stepSize, thickness);
        drawVerticalLine(x + stepSize, .5f, y, stepSize, thickness);
    }

    void drawHorizontalLine(float x, float y, float z, float width, float thickness)
    {
        GL.Begin(GL.QUADS);
        GL.Color(selectColor);
        GL.Vertex3(x, y, z - thickness / 2);
        GL.Vertex3(x + width, y, z - thickness / 2);
        GL.Vertex3(x + width, y, z + thickness / 2);
        GL.Vertex3(x, y, z + thickness / 2);
        GL.End();
    }

    void drawVerticalLine(float x, float y, float z, float length, float thickness)
    {
        GL.Begin(GL.QUADS);
        GL.Color(selectColor);
        GL.Vertex3(x - thickness / 2, y, z);
        GL.Vertex3(x - thickness / 2, y, z + length);
        GL.Vertex3(x + thickness / 2, y, z + length);
        GL.Vertex3(x + thickness / 2, y, z);
        GL.End();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLines : MonoBehaviour {
    public int tileStartX, tileStartZ;
    public int stepSize;
    public int mapSizeX, mapSizeZ;
    public Color color = new Color(158f/255f, 158f/255f, 158f/255f, 158f/255f);

    private float startX, startZ;

    void Start ()
    {
        startX = (float)tileStartX - (float)stepSize / 2;
        startZ = (float)tileStartZ - (float)stepSize / 2;
    }

    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    void OnPostRender ()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();

        // Draw lines
        for (int i = 0; i < mapSizeX; i++)
        {
            GL.Begin(GL.LINE_STRIP);
            GL.Color(color);
            for (int j = 0; j < mapSizeZ; j++)
            {
                GL.Vertex3(i * stepSize + startX, .5f, j * stepSize + startZ);
                GL.Vertex3((i + 1) * stepSize + startX, .5f, j * stepSize + startZ);
                GL.Vertex3((i + 1) * stepSize + startX, .5f, (j + 1) * stepSize + startZ);
                GL.Vertex3(i * stepSize + startX, .5f, (j + 1) * stepSize + startZ);
                GL.Vertex3(i * stepSize + startX, .5f, j * stepSize + startZ);
            }
            GL.End();
        }

        GL.PopMatrix();
    }
}

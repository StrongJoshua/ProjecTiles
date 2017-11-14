using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBuilder : MonoBehaviour {

    private GameObject head, body;

    private void Start()
    {
        foreach(LineRenderer lr in GetComponentsInChildren<LineRenderer>())
        {
            if (lr.gameObject.name == "Head")
                head = lr.gameObject;
            else if (lr.gameObject.name == "Body")
                body = lr.gameObject;
        }
    }

    public void setPath(List<Vector2> path)
    {
        Vector3[] positions = new Vector3[path.Count];
        print("Path (length = " + (positions.Length) + ")");
        for(int i = 0; i < path.Count; i++)
        {
            print(path[i]);
            positions[i] = new Vector3(path[i].x * MapGenerator.step, .7f, path[i].y * MapGenerator.step);
        }
        string s = "Setting positions to: ";
        foreach (Vector3 v in positions)
            s += v + " ";
        print(s);
        body.GetComponent<LineRenderer>().SetPositions(positions);
        head.transform.position = positions[positions.Length - 1];
    }
}

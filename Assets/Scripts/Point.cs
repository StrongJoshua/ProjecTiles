using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point {
    private int x;
    public int X
    {
        get
        {
            return this.x;
        }
    }
    private int y;
    public int Y
    {
        get
        {
            return this.y;
        }
    }

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return "(" + x + ", " + y + ")";
    }
}

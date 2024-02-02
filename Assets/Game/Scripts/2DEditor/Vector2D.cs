using System;
using System.Collections;
using System.Collections.Generic;

public class Vector2D: Object
{
    public int X;
    public int Y;

    public Vector2D(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override bool Equals(object obj)
    {
        var point = (Vector2D)obj;
        if (this.X == point.X && this.Y == point.Y)
            return true;
        return false;
    }
}

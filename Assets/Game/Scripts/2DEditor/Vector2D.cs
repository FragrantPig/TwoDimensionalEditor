using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

public class Vector2D: Object
{
    public int X;
    public int Y;

    public Vector2D()
    {
        X = 0;
        Y = 0;
    }
    
    public Vector2D(int x, int y)
         {
             X = x;
             Y = y;
         }
         
         public Vector2D(Vector2 vector2)
         {
             X = Mathf.RoundToInt(vector2.x);
             Y = Mathf.RoundToInt(vector2.y);
         }
         
         public Vector2D(Vector3 vector3)
         {
             X = Mathf.RoundToInt(vector3.x);
             Y = Mathf.RoundToInt(vector3.y);
         }
     
         public override bool Equals(object obj)
         {
             var point = (Vector2D)obj;
             if (this.X == point.X && this.Y == point.Y)
                 return true;
             return false;
         }
}

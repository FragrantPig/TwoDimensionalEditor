using UnityEngine;
using Object = System.Object;

namespace Game.Scripts._2DEditor
{
    public class Vector3D: Object
    {
        public int X;
        public int Y;
        public int Z;
        
        public Vector3D()
        {
            X = 0;
            Y = 0;
            Z = 0;
        }
        
        public Vector3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
         
        public Vector3D(Vector3 vector3)
        {
            X = Mathf.RoundToInt(vector3.x);
            Y = Mathf.RoundToInt(vector3.y);
            Z = Mathf.RoundToInt(vector3.z);
        }
     
        public override bool Equals(object obj)
        {
            var point = (Vector3D)obj;
            if (this.X == point.X && this.Y == point.Y && this.Z == point.Z)
                return true;
            return false;
        }
    }
}
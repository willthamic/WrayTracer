using System;
using Vector;

namespace Geometry
{
    public class Rectangle
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
        public Vector3 d;

        public Rectangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            a = p1;
            b = p2;
            c = p3;
            d = a + c - b;
        }

        public Vector3 Normal()
        {
            Vector3 u = b - a;
            Vector3 v = d - a;

            float u1 = u.x;
            float u2 = u.y;
            float u3 = u.z;
            float v1 = v.x;
            float v2 = v.y;
            float v3 = v.z;

            float x =   u.y*v.z-v.y*u.z;
            float y = -(u.x*v.z-v.x-u.z);
            float z =   u.x*v.y-v.x*u.y;
            return new Vector3(x, y, z).Unit();
        }



    }
}

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
        public Plane plane;

        public Rectangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            a = p1;
            b = p2;
            c = p3;
            d = a + c - b;
            plane = new Plane(a, b - a, d - a);


        }

        



    }
}

using System;
using Vector;

namespace Geometry
{
    public class Rectangle
    {
        Vector3 a;
        Vector3 b;
        Vector3 c;
        Vector3 d;

        public Rectangle(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            a = p1;
            b = p2;
            c = p3;
            d = a + c - b;
        }

    }
}

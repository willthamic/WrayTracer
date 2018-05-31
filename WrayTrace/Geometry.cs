using System;
using Vector;

namespace Geometry
{
    public class Parallelogram
    {
        public Triangle A;
        public Triangle B;

        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
        public Vector3 d;

        public Plane plane;

        public Parallelogram(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            a = p1;
            b = p2;
            c = p3;

            d = a + c - b;

            A = new Triangle(a, b, c);
            B = new Triangle(a, c, d);

            plane = A.plane;
        }

        public Vector3 FindIntersect(Vector3 o, Vector3 d)
        {
            Vector3 aIntersect = A.FindIntersect(o, d);
            Vector3 bIntersect = B.FindIntersect(o, d);
            if (aIntersect != null)
                return aIntersect;
            else if (bIntersect != null)
                return bIntersect;
            else
                return null;
        }
    }

    public class Triangle
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
        public Plane plane;
        
        public Triangle(Vector3 a0, Vector3 b0, Vector3 c0)
        {
            a = a0;
            b = b0;
            c = c0;
            plane = new Plane(a, b - a, c - a);
        }

        public Vector3 FindIntersect(Vector3 o, Vector3 d)
        {
            Vector3 p = Plane.RayCast(o, d, plane);
            if (p == null)
                return null;
            float i = (float) Math.Acos(Vector3.Dot((a - p).Unit(), (b - p).Unit()));
            float j = (float) Math.Acos(Vector3.Dot((b - p).Unit(), (c - p).Unit()));
            float k = (float) Math.Acos(Vector3.Dot((c - p).Unit(), (a - p).Unit()));
            if (i + j + k > 2 * Math.PI - 0.0001 || i + j + k == float.NaN)
                return p;
            else
                return null;
        }
    }

    public class Material
    {

    }
}

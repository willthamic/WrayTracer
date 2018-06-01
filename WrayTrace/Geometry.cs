using System;
using Vector;

namespace Geometry
{
     /*public class Element
    {
        public Object element;

        public Element(Object element0)
        {
            element = element0;
        }

        public (bool, Vector3, float) FindIntersect(Line line)
        {
            if (element is Parallelogram)
                return ((Parallelogram)element).FindIntersect(line);
            else if (element is Triangle)
                return ((Triangle)element).FindIntersect(line);
            else
                return (false, null, 0);
        }

        public Vector3 GetNormal()
        {
            if (element is Parallelogram)
                return ((Parallelogram)element).plane.normal;
            else if (element is Triangle)
                return ((Triangle)element).plane.normal;
            else
                return null;
        }
    }*/

    /// <summary>
    /// Represents a parallelogram defined by three points in 3-dimensions.
    /// </summary>
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

        /// <summary>
        /// Finds the intersection of the parallelogram and a specified line.
        /// </summary>
        public (bool, Vector3, float) FindIntersect(Line line)
        {
            (bool, Vector3, float) aIntersect = A.FindIntersect(line);
            (bool, Vector3, float) bIntersect = B.FindIntersect(line);
            if (aIntersect.Item1)
                return (true, aIntersect.Item2, aIntersect.Item3);
            else if (bIntersect.Item1)
                return (true, bIntersect.Item2, bIntersect.Item3);
            else
                return (false, null, 0);
        }
    }

    /// <summary>
    /// Represents a triangle defined by three points in 3-dimensions.
    /// </summary>
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

        /// <summary>
        /// Finds the intersection of the triangle and a specified line.
        /// </summary>
        public (bool, Vector3, float) FindIntersect(Line line)
        {
            (bool, Vector3, float) raycast = Plane.RayCast(line, plane);
            Vector3 p = raycast.Item2;
            if (p == null)
                return (false, null, 0);
            float i = (float) Math.Acos(Vector3.Dot((a - p).Unit(), (b - p).Unit()));
            float j = (float) Math.Acos(Vector3.Dot((b - p).Unit(), (c - p).Unit()));
            float k = (float) Math.Acos(Vector3.Dot((c - p).Unit(), (a - p).Unit()));
            if (i + j + k > 2 * Math.PI - 0.0001 || i + j + k == float.NaN)
                return (true, p, raycast.Item3);
            else
                return (false, null, 0);
        }
    }

    public class Material
    {

    }
}

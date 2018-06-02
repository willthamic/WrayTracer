using System;
using Vector;

namespace Geometry
{
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

        public Parallelogram Translate(Vector3 vector)
        {
            return new Parallelogram(this.a + vector, this.b + vector, this.c + vector);
        }

        public static Parallelogram operator +(Parallelogram parallelogram, Vector3 vector) {
            return parallelogram.Translate(vector);
        }

        public static Parallelogram operator +(Vector3 vector, Parallelogram parallelogram)
        {
            return parallelogram.Translate(vector);
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

    public class Paralleloid
    {
        public Vector3[] points = new Vector3[8];
        public Parallelogram[] faces = new Parallelogram[6];

        public Paralleloid (Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            points[0] = p1;
            points[1] = p2;
            points[2] = p2 + (p3 - p1);
            points[3] = p3;
            points[4] = p4;
            points[5] = p4 + (p2 - p1);
            points[6] = points[5] + (p3 - p1);
            points[7] = p3 + (p4 - p1);

            faces[0] = new Parallelogram(points[1], points[0], points[4]);
            faces[1] = new Parallelogram(points[3], points[0], points[4]);
            faces[2] = new Parallelogram(points[1], points[0], points[3]);
            faces[3] = faces[0] + (points[3] - points[0]);
            faces[4] = faces[1] + (points[1] - points[0]);
            faces[5] = faces[2] + (points[4] - points[0]);
        }
    }
}

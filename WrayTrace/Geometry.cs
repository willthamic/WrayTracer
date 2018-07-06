using System;
using System.Threading;
using Vector;

namespace Geometry
{
    public interface Element
    {
        (bool, Vector3, float, Vector3) Raycast(Line line, String type);
    }

    /// <summary>
    /// Represents a triangle defined by three points in 3-dimensions.
    /// </summary>
    public class Triangle : Element
    {
        public Vector3 a;
        public Vector3 b;
        public Vector3 c;
        public Vector3 ab;
        public Vector3 bc;
        public Vector3 ca;
        public Plane plane;
        
        public Triangle(Vector3 a0, Vector3 b0, Vector3 c0)
        {
            a = a0;
            b = b0;
            c = c0;

            ab = b - a;
            bc = c - b;
            ca = a - c;

            plane = new Plane(a, b - a, c - a);
        }

        public (bool, Vector3, float, Vector3) Raycast(Line line, String type)
        {
            return TriangleRaycast(this, line, type);
        }

        static (bool, Vector3, float, Vector3) TriangleRaycast(Triangle element, Line line, String type)
        {
            if (type == "IO")
                return IO(element, line);
            else
                return W(element, line);
        }

        static (bool, Vector3, float, Vector3) W(Triangle triangle, Line line)
        {
            (bool, Vector3, float) raycast = Plane.RayCast(line, triangle.plane);
            Vector3 p = raycast.Item2;
            if (p == null)
                return (false, null, 0, null);
            float i = (float)Math.Acos(Vector3.Dot((triangle.a - p).Unit(), (triangle.b - p).Unit()));
            float j = (float)Math.Acos(Vector3.Dot((triangle.b - p).Unit(), (triangle.c - p).Unit()));
            float k = (float)Math.Acos(Vector3.Dot((triangle.c - p).Unit(), (triangle.a - p).Unit()));
            if (i + j + k > 2 * Math.PI - 0.001 || i + j + k == float.NaN)
                return (true, p, raycast.Item3, triangle.plane.normal);
            else
                return (false, null, 0, null);
        }

        static (bool, Vector3, float, Vector3) IO(Triangle triangle, Line line)
        {
            (bool, Vector3, float) raycast = Plane.RayCast(line, triangle.plane);
            Vector3 p = raycast.Item2;
            if (p == null)
                return (false, null, 0, null);
            Vector3 ap = p - triangle.a;
            Vector3 bp = p - triangle.b;
            Vector3 cp = p - triangle.c;
            if (Vector3.Dot(triangle.plane.normal, Vector3.Normal(triangle.ab, ap)) > 0 &&
                Vector3.Dot(triangle.plane.normal, Vector3.Normal(triangle.bc, bp)) > 0 &&
                Vector3.Dot(triangle.plane.normal, Vector3.Normal(triangle.ca, cp)) > 0)
                return (true, p, raycast.Item3, triangle.plane.normal);
            else
                return (false, null, 0, null);
        }
    }

    /// <summary>
    /// Represents a parallelogram defined by three points in 3-dimensions.
    /// </summary>
    public class Parallelogram : Element
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

        public (bool, Vector3, float, Vector3) Raycast(Line line, String type)
        {
            var temp = this.A.Raycast(line, type);
            if (temp.Item1)
                return temp;
            else
                return this.B.Raycast(line, type);
        }
    }

    public class Paralleloid : Element
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

        public (bool, Vector3, float, Vector3) Raycast(Line line, String type)
        {
            (bool, Vector3, float, Vector3) intersect = (false, null, 0, null);
            foreach (Parallelogram parallelogram in faces)
            {
                var temp = parallelogram.Raycast(line, type);
                if (temp.Item1 && temp.Item3 > 0 && (!intersect.Item1 || temp.Item3 < intersect.Item3))
                    intersect = temp;
            }
            return intersect;
        }
    }
}

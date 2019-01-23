using System;
using System.Threading;
using Vector;

namespace Geometry
{
    public interface Element
    {
        (bool, Vector3, float, Vector3) Raycast(Line line);

        bool SimpleRaycast(Line line);
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

        public Triangle(Vector3 a, Vector3 b, Vector3 c)
        {
            this.a = a;
            this.b = b;
            this.c = c;

            ab = b - a;
            bc = c - b;
            ca = a - c;

            plane = new Plane(a, b - a, c - a);
        }

        public (bool, Vector3, float, Vector3) Raycast(Line line)
        {
            return TriangleRaycast(this, line);
        }

        public bool SimpleRaycast(Line line)
        {
            var temp = Raycast(line);
            return temp.Item1 && (temp.Item3 > 0);
        }

        static (bool, Vector3, float, Vector3) TriangleRaycast(Triangle element, Line line)
        {
            String type = "IO";
            if (type == "IO")
                return IO(element, line);
            else if (type == "MT")
                return MT(element, line);
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

        static (bool, Vector3, float, Vector3) MT(Triangle triangle, Line line)
        {
            const float epsilon = 0.00001f;
            Vector3 ab = triangle.b - triangle.a;
            Vector3 ac = triangle.c - triangle.a;
            Vector3 pVec = Vector3.Normal(line.direction, ac);
            float det = Vector3.Dot(ab, pVec);

            if (det > -epsilon && det < epsilon)
                return (false, null, 0, null);

            float f = 1.0f / det;
            Vector3 s = line.origin - triangle.a;
            float u = f * Vector3.Dot(s, pVec);

            if (u < 0.0 || u > 1.0)
                return (false, null, 0, null);

            Vector3 q = Vector3.Normal(s, ab);
            float t = f * Vector3.Dot(ac, q);

            if (t > epsilon)
                return (true, line.origin + t * line.direction, t, triangle.plane.normal);
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

        public Parallelogram(Vector3 origin, Vector3 v, Vector3 u)
        {
            a = origin;
            b = v;
            c = u;

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

        public (bool, Vector3, float, Vector3) Raycast(Line line)
        {
            var temp = this.A.Raycast(line);
            if (temp.Item1)
                return temp;
            else
                return this.B.Raycast(line);
        }

        public bool SimpleRaycast(Line line)
        {
            var temp = Raycast(line);
            return temp.Item1 && (temp.Item3 > 0);
        }
    }

    public class Parallelepiped : Element
    {
        public Vector3[] points = new Vector3[8];
        public Parallelogram[] faces = new Parallelogram[6];

        public Parallelepiped (Vector3 origin, Vector3 v, Vector3 u, Vector3 w)
        {
            points[0] = origin;
            points[1] = v;
            points[2] = v + (u - origin);
            points[3] = u;
            points[4] = w;
            points[5] = w + (v - origin);
            points[6] = points[5] + (u - origin);
            points[7] = u + (w - origin);

            faces[0] = new Parallelogram(points[1], points[0], points[4]);
            faces[1] = new Parallelogram(points[3], points[0], points[4]);
            faces[2] = new Parallelogram(points[1], points[0], points[3]);
            faces[3] = faces[0] + (points[3] - points[0]);
            faces[4] = faces[1] + (points[1] - points[0]);
            faces[5] = faces[2] + (points[4] - points[0]);
        }

        public (bool, Vector3, float, Vector3) Raycast(Line line)
        {
            (bool, Vector3, float, Vector3) intersect = (false, null, 0, null);
            foreach (Parallelogram parallelogram in faces)
            {
                var temp = parallelogram.Raycast(line);
                if (temp.Item1 && temp.Item3 > 0 && (!intersect.Item1 || temp.Item3 < intersect.Item3))
                    intersect = temp;
            }
            return intersect;
        }

        public bool SimpleRaycast(Line line)
        {
            return Raycast(line).Item1;
        }
    }

    public class Cube : Element
    {

        public Vector3 center;
        public float radius;
        public float theta;
        public float phi;

        public Sphere inside;
        public Sphere outside;
        public Parallelepiped self;
            

        public Cube (Vector3 center, float side)
        {
            this.center = center;
            this.radius = side / 2;
            theta = 0;
            phi = 0;
            inside = new Sphere(center, radius);
            outside = new Sphere(center, radius * (float) Math.Sqrt(3));
            self = new Parallelepiped(center - new Vector3(radius, radius, radius), new Vector3(2*radius, 0, 0), new Vector3(0, 2 * radius, 0), new Vector3(0, 0, 2 * radius));
        }

        public (bool, Vector3, float, Vector3) Raycast(Line line)
        {
            //if (outside.SimpleRaycast(line) == false)
            //    return (false, null, 0, null);

            if (theta == 0 && phi == 0)
            {
                
                return (false, null, 0, null);
            }

            return (false, null, 0, null); // TO BE FIXED
        }

        public bool SimpleRaycast(Line line)
        {
            //if (inside.SimpleRaycast(line))
            //    return true;

            var temp = Raycast(line);
            return temp.Item1 && (temp.Item3 > 0);
        }
    }


    public class Sphere : Element
    {
        Vector3 center;
        float radius;

        public Sphere (Vector3 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }


        public (bool, Vector3, float, Vector3) Raycast(Line line)
        {
            // 0 = (x - center.x) ^ 2 + (y - center.y) ^ 2 + (z - center.z) ^ 2 - radius^2;
            // x = line.origin.x + line.direction.x * t;
            // y = line.origin.y + line.direction.y * t;
            // z = line.origin.z + line.direction.z * t;
            
            // 0 = (line.origin.x + line.direction.x * t - center.x) ^ 2 + (line.origin.y + line.direction.y * t - center.y) ^ 2 + (line.origin.z + line.direction.z * t - center.z) ^ 2 - radius^2;

            Vector3 a = line.origin - center;
            Vector3 b = line.direction;

            // 0 = (a.x + b.x * t) ^ 2 + (a.y + b.y * t) ^ 2 + (a.z + b.z * t) ^ 2 - radius^2;

            // 0 = a.x * a.x + 2 * a.x * b.x * t + b.x * b.x * t * t
            //   + a.y * a.y + 2 * a.y * b.y * t + b.y * b.y * t * t
            //   + a.z * a.z + 2 * a.z * b.z * t + b.z * b.z * t * t
            //   - radius^2

            // 0 = A + B * t + C * t^2
            //float A = a.x * a.x + a.y + a.y + a.z + a.z - radius * radius;
            //float B = 2 * a.x * b.x + 2 * a.y * b.y + 2 * a.z * b.z;
            //float C = b.x * b.x + b.y * b.y + b.z * b.z;

            float A = Vector3.Dot(b, b);
            float B = 2 * Vector3.Dot(a, b);
            float C = Vector3.Dot(a, a) - radius * radius;

            float descr = B * B - 4 * A * C;
            if (descr >= 0)
            {
                float sqrtDescr = (float)Math.Sqrt(descr);
                float t1 = (-B + sqrtDescr) / (2 * A) - 0.000001f;
                float t2 = (-B - sqrtDescr) / (2 * A) - 0.000001f;

                if (t1 > 0 && t2 > 0 )
                {
                    if (t1 < t2)
                    {
                        Vector3 p = line.origin + line.direction * t1;
                        return (true, p, t1, p - center);
                    }
                    else
                    {
                        Vector3 p = line.origin + line.direction * t2;
                        return (true, p, t2, p - center);
                    }
                }
                else if (t1 > 0 && t2 <= 0)
                {
                    Vector3 p = line.origin + line.direction * t1;
                    return (true, p, t1, p - center);
                }
                else if (t1 <= 0 && t2 > 0)
                {
                    Vector3 p = line.origin + line.direction * t2;
                    return (true, p, t2, p - center);
                }
                else
                {
                    return (false, null, 0, null);
                }
            }
            else
            {
                return (false, null, 0, null);
            }

        }

        public bool SimpleRaycast(Line line)
        {
            Vector3 a = line.origin - center;
            Vector3 b = line.direction;

            float A = Vector3.Dot(b, b);
            float B = 2 * Vector3.Dot(a, b);
            float C = Vector3.Dot(a, a) - radius * radius;

            float descr = B * B - 4 * A * C;
            return (descr >= 0) && (((-B + Math.Sqrt(descr)) / (2 * A) - 0.000001f) > 0 || ((-B + Math.Sqrt(descr)) / (2 * A) - 0.000001f) > 0);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Vector;
using Geometry;

namespace WrayTrace
{
    class Program
    {
        static void Main(string[] args)
        {
            int temp = 1000;
            Bitmap bitmap = new Bitmap(temp, temp);

            Vector3 a = new Vector3(-1, 1, 0);
            Vector3 b = new Vector3(-1, -1, 0);
            Vector3 c = new Vector3(1, -1, 0);
            Geometry.Parallelogram border = new Geometry.Parallelogram(a, b, c);
            Sensor sensor = new Sensor(border, temp, temp);
            Camera camera = new Camera(new Vector3(0, 0, 1),sensor);

            Vector3 a0 = new Vector3(-1, 1, -1.5f);
            Vector3 b0 = new Vector3(-1, -1, -1);
            Vector3 c0 = new Vector3(1, -1, -1);
            Geometry.Parallelogram rect = new Geometry.Parallelogram(a0, b0, c0);

            Light light = new Light(new Vector3(0.5f, 0.5f, -0.5f), 200);

            for (var i = 0; i < temp; i++)
            {
                for (var j = 0; j < temp; j++)
                {
                    Line ray = new Line(camera.location, camera.sensor.LocatePixel(i, j) - camera.location);
                    Vector3 intersect = rect.FindIntersect(ray);
                    if (intersect != null)
                    {
                        int intensity = Convert.ToInt32(Vector3.Dot(rect.plane.normal.Unit(), (light.location - intersect).Unit()) * light.intensity);
                        bitmap.SetPixel(i, j, Color.FromArgb(intensity, intensity, intensity));
                    }
                }
            }

            bitmap.Save("img.bmp");
        }
    }

    class Camera
    {
        public Vector3 location;
        public Sensor sensor;

        public Camera (Vector3 location0, Sensor sensor0)
        {
            location = location0;
            sensor = sensor0;
        }
    }

    class Sensor
    {
        Geometry.Parallelogram border;
        int pWidth;
        int pHeight;

        public Sensor(Geometry.Parallelogram border0, int pWidth0, int pHeight0)
        {
            border = border0;
            pWidth = pWidth0;
            pHeight = pHeight0;
        }

        public Vector3 LocatePixel(int x, int y)
        {
            Vector3 ad = border.d - border.a;
            Vector3 ab = border.b - border.a;
            Vector3 ap = (ad / (pWidth - 1) * x) + (ab / (pHeight - 1) * y);
            return border.a + ap;
        }
    }

    class Light
    {
        public Vector3 location;
        public float intensity;

        public Light(Vector3 location0, float intensity0)
        {
            location = location0;
            intensity = intensity0;
        }
    }
}
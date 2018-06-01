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
            int width = 1920;
            int height = 1080;

            Bitmap bitmap = new Bitmap(width, height);

            Parallelogram sensorBorder = new Parallelogram(V(-1.6f, 1, 0), V(-1.6f, -1, 0), V(1.6f, -1, 0));
            Sensor sensor = new Sensor(sensorBorder, width, height);
            Camera camera = new Camera(V(0, 0, 1), sensor);

            Parallelogram rect1 = new Parallelogram(V(-1.5f, 1.5f, -1.5f), V(-1.5f, -1.5f, -1), V(1.5f, -1.5f, -1));
            Parallelogram rect2 = new Parallelogram(V(-.5f, .5f, -.7f), V(-.5f, -.5f, -.8f), V(.5f, -.5f, -.8f));



            Triangle[] elements = { rect1.A, rect1.B, rect2.A, rect2.B };


            Light light = new Light(new Vector3(0.5f, 0.5f, 0f), 200);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    if (x == 400 && y == 400)
                    {
                        var asdasd = 0;
                    }
                    Line ray = new Line(camera.location, camera.sensor.LocatePixel(x, y) - camera.location);

                    Triangle intersectedObject = null;
                    Vector3 p = null;
                    float minT = 0;

                    bool flag = true;

                    foreach (Triangle element in elements) {
                        (bool, Vector3, float) intersect = element.FindIntersect(ray);

                        if (intersect.Item1 && (flag || (intersect.Item3 < minT && intersect.Item3 > 0)))
                        {
                            flag = false;
                            intersectedObject = element;
                            p = intersect.Item2;
                            minT = intersect.Item3;
                        }
                    }

                    if (p != null)
                    {
                        bool clearpath = true;

                        foreach (Triangle element in elements)
                        {
                            if (element == intersectedObject)
                                continue;
                            (bool, Vector3, float) intersect = element.FindIntersect(new Line(p, light.location - p));
                            if (intersect.Item1 && intersect.Item3 > 0)
                            {
                                clearpath = false;
                                break;
                            }
                        }

                        if (clearpath)
                        {
                            int intensity = Convert.ToInt32(Vector3.Dot(intersectedObject.plane.normal.Unit(), (light.location - p).Unit()) * light.intensity);
                            bitmap.SetPixel(x, y, Color.FromArgb(intensity, intensity, intensity));
                        }
                    }
                }
            }

            bitmap.Save("img.bmp");
        }

        static Vector3 V(float x0, float y0, float z0)
        {
            return new Vector3(x0, y0, z0);
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
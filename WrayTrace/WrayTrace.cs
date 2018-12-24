using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Vector;
using Geometry;
using System.Diagnostics;
using System.Threading;

namespace WrayTrace
{
            /*
    class WrayTrace
    {
        static void Main(string[] args)
        { 
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Console.Write("Generating Scene...");

            int width = 1920/4;
            int height = 1080/4;

            Bitmap image = new Bitmap(width, height);
            float[,] imageValues = new float[width, height];
            //for (int x = 0; x < width; x++)
            //{
            //    for (int y = 0; y < height; y++)
            //    {
            //        image.SetPixel(x, y, Color.Black);
            //    }
            //}

            //Parallelogram sensorBorder = new Parallelogram(V(-1, -4, 4), V(-1, -4, 2), V(1, -4, 2));
            //Sensor sensor = new Sensor(sensorBorder, width, height);
            //Camera camera = new Camera(V(-2, 4, 4) * 1.5f, V(.5f, -1, -.75f).Unit(), 1.2f, width, height);
            Camera camera = new Camera(V(2, -4, 4) * 1.5f, V(-.5f, 1, -.75f).Unit(), 1.2f, width, height);
            //Camera camera = new Camera(V(0, 0, 2), V(0.1f, 0, -1), 1.2f, width, height);

            Parallelogram floor = new Parallelogram(V(-10, 10, 0), V(-10, -10, 0), V(10, -10, 0));
            Paralleloid cube = new Paralleloid(V(1, 1, 0), V(1, -1, 0), V(-1, 1, 0), V(1, 1, 2));
            Paralleloid cube2 = new Paralleloid(V(0.866025404f, 0.5f, 2), V(0.5f, -0.866025404f, 2), V(-0.5f, 0.866025404f, 2), V(0.866025404f, 0.5f, 3.41f));

            Light light1 = new Light(new Vector3(20, -14, 30), 4000);
            Light light2 = new Light(new Vector3(15, -14, 30), 5000);


            List<Element> elements0 = new List<Element>();
            elements0.Add(floor);
            elements0.Add(cube);
            elements0.Add(cube2);

            List<Light> lights = new List<Light>();
            lights.Add(light1);

            Console.Write("\rGenerating Scene [Done - {0}s]\r\n", 0.001 * stopwatch.ElapsedMilliseconds);

            int count = 0;

            Scene scene = new Scene(camera);
            scene.elements = elements0;
            scene.lights = lights;

            var imageColors = scene.Render();


            Console.Write("\rGenerating Image [Done - {0}s]\n\r", 0.001 * stopwatch.ElapsedMilliseconds);
            Console.Write("Saving Image...");

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    image.SetPixel(x, y, imageColors[x, y]);
                }
            }

            image.Save("img.bmp");
            Console.Write("\rSaving Image [Done - {0}s]\n\r", 0.001 * stopwatch.ElapsedMilliseconds);
        }

        static Vector3 V(float x0, float y0, float z0)
        {
            return new Vector3(x0, y0, z0);
        }
    }
    */

    public class Scene
    {
        public Camera camera;
        public List<Light> lights;
        public List<Element> elements;
        public int width;
        public int height;

        public Scene(Camera camera)
        {
            this.camera = camera;
            width = camera.sensor.pWidth;
            height = camera.sensor.pHeight;
        }

        public Color[,] Render(ref String status)
        {
            Color[,] imageColors = new Color[width, height];
            float [,] imageValues = GetIntensityValues(ref status);
            float maxIntensity = (from float v in imageValues select v).Max();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    
                    int intensity = Convert.ToInt32(imageValues[x, y] / maxIntensity * 255);
                    imageColors[x, y] = Color.FromArgb(intensity, intensity, intensity);
                }
            }
            return imageColors;
        }

        public float[,] GetIntensityValues(ref String status)
        {
            float[,] intensityValues = new float[width, height];

            //for (int i = 0; i < width*height; i++)
            Parallel.For(0, width * height, (i) =>
            {
                int xIndex = i % width;
                int yIndex = i / width;
                intensityValues[xIndex, yIndex] = GetPixelIntensity(xIndex, yIndex);
            }
            );
            return intensityValues;
        }

        public float GetPixelIntensity(int xIndex, int yIndex)
        {
            float intensity = 0;

            Line ray = new Line(camera.location, camera.sensor.LocatePixel(xIndex, yIndex) - camera.location);

            Element intersectedObject = null;
            Vector3 p = null;
            float minT = 0;
            Vector3 surfaceNormal = null;

            bool flag = true;

            // Finds the first element that the ray intersects with.
            foreach (Element element in elements)
            {
                var intersect = element.Raycast(ray);
                if (intersect.Item1 && ((flag || intersect.Item3 < minT) && intersect.Item3 > 0))
                {
                    flag = false;
                    intersectedObject = element;
                    p = intersect.Item2;
                    minT = intersect.Item3;
                    surfaceNormal = intersect.Item4;
                }
            }

            // Determines the intensity of the light
            if (intersectedObject != null) {
                foreach (Light light in lights)
                {
                    bool clearpath = true;

                    foreach (Element element in elements)
                    {
                        //if (element == intersectedObject)
                        //    continue;
                        var intersect = element.Raycast(new Line(p, light.location - p));

                        if (intersect.Item1 && intersect.Item3 > 0)
                        {
                            clearpath = false;
                            break;
                        }

                    }
                    if (clearpath)
                    {
                        intensity += Math.Abs(light.GetIntensity(surfaceNormal, p));
                    }
                }
            }
            return intensity;
        }
    }
    
    public class Camera
    {
        public Vector3 location;
        public Sensor sensor;

        public Camera (Vector3 location0, Sensor sensor0)
        {
            location = location0;
            sensor = sensor0;
        }

        public Camera(Vector3 location0, Vector3 direction, float width, int pWidth, int pHeight)
        {
            location = location0;
            float height = width / pWidth * pHeight;
            Vector3 center = location + direction;
            Vector3 hOffset = Vector3.Normal(direction, new Vector3(0, 0, 1)).Unit() * width / 2;
            Vector3 vOffset = Vector3.Normal(direction, hOffset).Unit() * height / 2;
            Parallelogram border = new Parallelogram(center - hOffset - vOffset, center - hOffset + vOffset, center + hOffset + vOffset);
            sensor = new Sensor(border, pWidth, pHeight);
        }
    }

    public class Sensor
    {
        Geometry.Parallelogram border;
        public int pWidth;
        public int pHeight;

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

    public class Light
    {
        public Vector3 location;
        public float intensity;

        public Light(Vector3 location0, float intensity0)
        {
            location = location0;
            intensity = intensity0;
        }

        public float GetIntensity (Vector3 surfaceNormal, Vector3 p)
        {
            return Vector3.Dot(surfaceNormal.Unit(), (location - p).Unit()) * intensity / (location - p).Magnitude();
        }
    }

    public class Spotlight : Light
    {
        float minAngle;
        float maxAngle;
        float minCosAngle;
        float maxCosAngle;

        public Spotlight(Vector3 location0, float intensity0, float angle) : this(location0, intensity0, angle, angle) {}

        public Spotlight(Vector3 location0, float intensity0, float minAngle0, float maxAngle0) : base(location0, intensity0)
        {
            location = location0;
            intensity = intensity0;
            minAngle = minAngle0;
            maxAngle = maxAngle0;
            minCosAngle = (float) Math.Cos(minAngle);
            maxCosAngle = (float) Math.Cos(maxAngle);
        }
    }
}
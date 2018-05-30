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
            int temp = 500;
            Bitmap bitmap = new Bitmap(temp, temp);

            Vector3 a = new Vector3(-1, 1, 0.3f);
            Vector3 b = new Vector3(-1, -1, 0);
            Vector3 c = new Vector3(1, -1, 0);
            Geometry.Rectangle border = new Geometry.Rectangle(a, b, c);
            Sensor sensor = new Sensor(border, temp, temp);
            Camera camera = new Camera(new Vector3(0, 0, 1),sensor);

            Vector3 a0 = new Vector3(-1, 1, -1);
            Vector3 b0 = new Vector3(-1, -1, -1);
            Vector3 c0 = new Vector3(1, -1, -1);
            Geometry.Rectangle rect = new Geometry.Rectangle(a0, b0, c0);

            for (var i = 0; i < temp; i++)
            {
                for (var j = 0; j < temp; j++)
                {
                    Vector3 impact = Vector3.RayCast(camera.location, camera.sensor.LocatePixel(i, j) - camera.location, rect.plane);
                    if (Math.Abs(impact.x) <= 1 && Math.Abs(impact.y) <= 1)
                    {
                        bitmap.SetPixel(i, j, Color.Black);
                    } else
                    {
                        bitmap.SetPixel(i, j, Color.White);
                    }
                }
            }

            bitmap.Save("img.bmp");
        }
    }

    class Camera
    {
        public Vector3 location;
        //Vector3 direction;
        public Sensor sensor;

        public Camera (Vector3 location0, Sensor sensor0)
        {
            location = location0;
            //direction = direction0;
            sensor = sensor0;
        }
    }

    class Sensor
    {
        Geometry.Rectangle border;
        int pWidth;
        int pHeight;

        public Sensor(Geometry.Rectangle border0, int pWidth0, int pHeight0)
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
}
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
            Bitmap image = new Bitmap(100, 100);

            Vector3 a = new Vector3(-1, 1, 0);
            Vector3 b = new Vector3(-1, -1, 0);
            Vector3 c = new Vector3(1, -1, 0);

            Geometry.Rectangle border = new Geometry.Rectangle(a, b, c);

            Sensor sensor = new Sensor(border, 5, 5);

            Console.WriteLine(border.Normal().ToString());
        }
    }

    class Camera
    {
        Vector3 location;
        //Vector3 direction;
        Sensor sensor;

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
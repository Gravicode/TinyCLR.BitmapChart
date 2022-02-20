using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace BitmapChart
{
    public class Point
    {
        public Point()
        {

        }
        public Point(int ax, int ay)
        {
            this.X = ax;
            this.Y = ay;

        }
        public int X { get; set; }
        public int Y { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace arcanoid.GameLogic
{
    class RectangleObject : DisplayObject
    {
        public Double Width { get; set; }
        public Double Height { get; set; }
        RectangleObject(double x, double y, double width, double height, double speed, double angle)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Speed = speed;
            Angle = angle;
            color = new SolidColorBrush(Color.FromRgb((byte)new Random().Next(256), (byte)new Random().Next(256), (byte)new Random().Next(256)));
        }
    }
}

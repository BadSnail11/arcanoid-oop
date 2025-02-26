using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace arcanoid.GameLogic
{
    class TrapezoidObject : DisplayObject
    {
        private Polygon trapezoid;

        public TrapezoidObject(double x, double y, double width, double height, double speed, double angle)
        {
            X = x;
            Y = y;
            Speed = speed;
            Angle = angle;
            color = new SolidColorBrush(Color.FromRgb((byte)new Random().Next(256), (byte)new Random().Next(256), (byte)new Random().Next(256)));

            trapezoid = new Polygon
            {
                Fill = color,
                Points = new PointCollection
                {
                    new System.Windows.Point(width * 0.25, 0),
                    new System.Windows.Point(width * 0.75, 0),
                    new System.Windows.Point(width, height),
                    new System.Windows.Point(0, height)
                }
            };
        }

        public override void Draw(Canvas canvas)
        {
            if (!canvas.Children.Contains(trapezoid))
                canvas.Children.Add(trapezoid);
            Canvas.SetLeft(trapezoid, X);
            Canvas.SetTop(trapezoid, Y);
        }
    }
}

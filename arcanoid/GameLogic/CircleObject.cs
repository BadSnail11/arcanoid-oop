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
    class CircleObject : DisplayObject
    {
        private Ellipse circle;
        public double Radius { get; set; }

        public CircleObject(double x, double y, double radius, double speed, double angle, double acceleration, double accelAngle)
        {
            X = x;
            Y = y;
            Radius = radius;
            Speed = speed;
            Angle = angle;
            Acceleration = acceleration;
            AccelAngle = accelAngle;
            color = new SolidColorBrush(Color.FromRgb((byte)new Random().Next(256), (byte)new Random().Next(256), (byte)new Random().Next(256)));

            circle = new Ellipse
            {
                Width = Radius * 2,
                Height = Radius * 2,
                Fill = color,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
        }

        public override void Draw(Canvas canvas)
        {
            if (!canvas.Children.Contains(circle))
                canvas.Children.Add(circle);
            Canvas.SetLeft(circle, X - Radius);
            Canvas.SetTop(circle, Y - Radius);
        }
    }
}

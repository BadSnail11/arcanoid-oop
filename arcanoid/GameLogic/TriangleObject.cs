using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace arcanoid.GameLogic
{
    class TriangleObject : DisplayObject
    {
        private Polygon triangle;
        public TriangleObject(double x, double y, double size, Color color, double speed, double angle, double acceleration, double accelAngle)
        {
            X = x;
            Y = y;
            Speed = speed;
            Angle = angle;
            Acceleration = acceleration;
            AccelAngle = accelAngle;
            //color = new SolidColorBrush(Color.FromRgb((byte)new Random().Next(256), (byte)new Random().Next(256), (byte)new Random().Next(256)));
            this.color = new SolidColorBrush(color);

            triangle = new Polygon
            {
                Fill = this.color,
                Points = new PointCollection
                {
                    new System.Windows.Point(0, 0),
                    new System.Windows.Point(size, size/2),
                    new System.Windows.Point(0, size)
                },
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
        }
        public override string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
        public override void Draw(Canvas canvas)
        {
            if (!canvas.Children.Contains(triangle))
                canvas.Children.Add(triangle);
            Canvas.SetLeft(triangle, X);
            Canvas.SetTop(triangle, Y);
        }
    }
}

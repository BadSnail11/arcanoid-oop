using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace arcanoid.GameLogic
{
    class RectangleObject : DisplayObject
    {
        public Double Width { get; set; }
        public Double Height { get; set; }
        private Rectangle rectangle;
        public RectangleObject(double x, double y, double width, double height, double speed, double angle, double acceleration, double accelAngle)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Speed = speed;
            Angle = angle;
            Acceleration = acceleration;
            AccelAngle = accelAngle;
            color = new SolidColorBrush(Color.FromRgb((byte)new Random().Next(256), (byte)new Random().Next(256), (byte)new Random().Next(256)));

            rectangle = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = color,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
        }
        public override void Draw(Canvas canvas)
        {
            if (!canvas.Children.Contains(rectangle))
                canvas.Children.Add(rectangle);
            Canvas.SetLeft(rectangle, X);
            Canvas.SetTop(rectangle, Y);
        }
    }
}

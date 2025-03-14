using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace arcanoid.GameLogic
{
    class TriangleObject : DisplayObject
    {
        public double Size { get; set; }

        [JsonIgnore]
        private Polygon triangle;

        private void initTriang()
        {
            triangle = new Polygon
            {
                Fill = this.color,
                Points = new PointCollection
                {
                    new System.Windows.Point(0, 0),
                    new System.Windows.Point(this.Size, this.Size/2),
                    new System.Windows.Point(0, this.Size)
                },
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
        }
        public TriangleObject() { }
        public TriangleObject(double x, double y, double size, Color color, double speed, double angle, double acceleration, double accelAngle)
        {
            X = x;
            Y = y;
            Size = size;
            Speed = speed;
            Angle = angle;
            Acceleration = acceleration;
            AccelAngle = accelAngle;
            //color = new SolidColorBrush(Color.FromRgb((byte)new Random().Next(256), (byte)new Random().Next(256), (byte)new Random().Next(256)));
            this.color = new SolidColorBrush(color);
            initTriang();
        }
        public override void ChangeBorder(Color color, double borderSize = 1)
        {
            if (triangle == null)
                initTriang();
            triangle.Stroke = new SolidColorBrush(color);
            triangle.StrokeThickness = borderSize;
        }
        public override string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
        public override DisplayObject FromJson(string json)
        {
            return JsonSerializer.Deserialize<TriangleObject>(json);
        }
        public override void Draw(Canvas canvas)
        {
            if (triangle == null)
            {
                initTriang();
            }
            if (!canvas.Children.Contains(triangle))
                canvas.Children.Add(triangle);
            Canvas.SetLeft(triangle, X);
            Canvas.SetTop(triangle, Y);
        }
    }
}

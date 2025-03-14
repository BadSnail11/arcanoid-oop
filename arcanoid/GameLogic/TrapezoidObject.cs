using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace arcanoid.GameLogic
{
    [Serializable]
    class TrapezoidObject : DisplayObject
    {
        public double Width { get; set; }
        public double Height { get; set; }
        [JsonIgnore]
        private Polygon trapezoid;

        private void initTrap()
        {
            trapezoid = new Polygon
            {
                Fill = this.color,
                Points = new PointCollection
                {
                    new System.Windows.Point(this.Width * 0.25, 0),
                    new System.Windows.Point(this.Width * 0.75, 0),
                    new System.Windows.Point(this.Width, this.Height),
                    new System.Windows.Point(0, this.Height)
                },
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
        }
        public TrapezoidObject() { }
        public TrapezoidObject(double x, double y, double width, double height, Color color, double speed, double angle, double acceleration, double accelAngle)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Speed = speed;
            Angle = angle;
            Acceleration = acceleration;
            AccelAngle = accelAngle;
            //color = new SolidColorBrush(Color.FromRgb((byte)new Random().Next(256), (byte)new Random().Next(256), (byte)new Random().Next(256)));
            this.color = new SolidColorBrush(color);
            initTrap();
            
        }
        public override void ChangeBorder(Color color, double borderSize = 1)
        {
            if (trapezoid == null)
                initTrap();
            trapezoid.Stroke = new SolidColorBrush(color);
            trapezoid.StrokeThickness = borderSize;
        }
        public override string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
        public override DisplayObject FromJson(string json)
        {
            return JsonSerializer.Deserialize<TrapezoidObject>(json);
        }
        public override void Draw(Canvas canvas)
        {
            if (trapezoid == null)
            {
                initTrap();
            }
            if (!canvas.Children.Contains(trapezoid))
                canvas.Children.Add(trapezoid);
            Canvas.SetLeft(trapezoid, X);
            Canvas.SetTop(trapezoid, Y);
        }
    }
}

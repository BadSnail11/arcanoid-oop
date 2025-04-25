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
    class CircleObject : DisplayObject
    {
        [JsonIgnore]
        private Ellipse circle;
        public double Radius { get; set; }

        private void initCircle()
        {
            circle = new Ellipse
            {
                Width = Radius * 2,
                Height = Radius * 2,
                Fill = this.color,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
        }
        public CircleObject() { }
        public CircleObject(double x, double y, double radius, Color color, double speed, double angle, double acceleration, double accelAngle)
        {
            X = x;
            Y = y;
            Radius = radius;
            HitX = X;
            HitY = Y;
            HitW = Radius * 2;
            HitH = Radius * 2;
            Speed = speed;
            Angle = angle;
            Acceleration = acceleration;
            AccelAngle = accelAngle;
            //color = new SolidColorBrush(Color.FromRgb((byte)new Random().Next(256), (byte)new Random().Next(256), (byte)new Random().Next(256)));
            this.color = new SolidColorBrush(color);
            initCircle();
            
        }
        public override void ChangeBorder(Color color, double borderSize = 1)
        {
            if (circle == null)
                initCircle();
            circle.Stroke = new SolidColorBrush(color);
            circle.StrokeThickness = borderSize;
        }
        public override string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
        public override DisplayObject FromJson(string json)
        {
            return JsonSerializer.Deserialize<CircleObject>(json);
        }
        public override void Draw(Canvas canvas)
        {
            if (circle == null)
            {
                initCircle();
            }
            if (!canvas.Children.Contains(circle))
                canvas.Children.Add(circle);
            Canvas.SetLeft(circle, X - Radius);
            Canvas.SetTop(circle, Y - Radius);
            // Отображение красной точки в центре круга
            //Ellipse centerDot = new Ellipse
            //{
            //    Width = 4,
            //    Height = 4,
            //    Fill = Brushes.Red
            //};
            //Canvas.SetLeft(centerDot, X - 2);
            //Canvas.SetTop(centerDot, Y - 2);
            //canvas.Children.Add(centerDot);

            //Rectangle hitbox = new Rectangle
            //{
            //    Width = HitW,
            //    Height = HitH,
            //    Stroke = Brushes.Red,
            //    StrokeDashArray = new DoubleCollection { 2 },
            //    StrokeThickness = 1
            //};
            //Canvas.SetLeft(hitbox, HitX - Radius);
            //Canvas.SetTop(hitbox, HitY - Radius);
            //canvas.Children.Add(hitbox);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace arcanoid.GameLogic
{
    abstract class DisplayObject
    {
        public Double X { get; set; }
        public Double Y { get; set; }
        public Double Speed { get; set; }
        public Double Angle { get; set; }
        public Double Acceleration { get; set; }
        public Double AccelAngle { get; set; }

        [JsonIgnore]
        public SolidColorBrush color { get; set; }
        public int[] ColorArray
        {
            get => new int[] { color.Color.R, color.Color.G, color.Color.B };
            set => color = new SolidColorBrush(Color.FromRgb((byte)value[0], (byte)value[1], (byte)value[2]));
        }

        public event Action<DisplayObject> OnClick;
        protected void RaiseClickEvent()
        {
            OnClick?.Invoke(this);
        }
        public virtual string ToJson()
        {
            return JsonSerializer.Serialize(this, GetType());
        }

        public static DisplayObject FromJson(string json, string type)
        {
            return type switch
            {
                "RectangleObject" => JsonSerializer.Deserialize<RectangleObject>(json),
                "TriangleObject" => JsonSerializer.Deserialize<TriangleObject>(json),
                "TrapezoidObject" => JsonSerializer.Deserialize<TrapezoidObject>(json),
                "CircleObject" => JsonSerializer.Deserialize<CircleObject>(json),
                _ => null
            };
        }
        public abstract void Draw(Canvas canvas);
        public virtual void Move(double width, double height, bool useAcceleration = false)
        {
            double radians = Angle * Math.PI / 180;
            double vx = Speed * Math.Cos(radians);
            double vy = Speed * Math.Sin(radians);

            if (useAcceleration)
            {
                double accelRadians = AccelAngle * Math.PI / 180;
                vx += Acceleration / 20 * Math.Cos(accelRadians);
                vy += Acceleration / 20 * Math.Sin(accelRadians);

                Speed = Math.Sqrt(vx * vx + vy * vy);
                if (Speed > 6)
                    Speed = 6;
                Angle = Math.Atan2(vy, vx) * 180 / Math.PI;
            }

            X += vx;
            Y += vy;

            // Проверка границ и отражение
            if (X < 0) { X = 0; Angle = 180 - Angle; }
            if (X > width) { X = width; Angle = 180 - Angle; }
            if (Y < 0) { Y = 0; Angle = -Angle; }
            if (Y > height) { Y = height; Angle = -Angle; }
        }
    }
}

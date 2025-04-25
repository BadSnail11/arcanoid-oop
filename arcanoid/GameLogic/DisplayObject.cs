using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

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
        public Double HitX { get; set; }
        public Double HitY { get; set; }
        public Double HitW { get; set; }
        public Double HitH { get; set; }
        public Rect HitBox
        {
            get => new Rect(HitX, HitY, HitW, HitH);
        }

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

        public abstract DisplayObject FromJson(string json);

        //public static DisplayObject FromJson(string json, string type)
        //{
        //    return type switch
        //    {
        //        "RectangleObject" => JsonSerializer.Deserialize<RectangleObject>(json),
        //        "TriangleObject" => JsonSerializer.Deserialize<TriangleObject>(json),
        //        "TrapezoidObject" => JsonSerializer.Deserialize<TrapezoidObject>(json),
        //        "CircleObject" => JsonSerializer.Deserialize<CircleObject>(json),
        //        _ => null
        //    };
        //}
        public abstract void ChangeBorder(Color color, double borderSize = 1);
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
                if (Speed > 40)
                    Speed = 40;
                Angle = Math.Atan2(vy, vx) * 180 / Math.PI;
            }

            X += vx;
            Y += vy;
            HitX += vx;
            HitY += vy;

            // Проверка границ и отражение
            if (X < 0) { X = 0; HitX = 0 ; Angle = 180 - Angle; }
            if (X > width) { X = width; HitX = width; Angle = 180 - Angle; }
            if (Y < 0) { Y = 0; HitY = 0 ; Angle = -Angle; }
            if (Y > height) { Y = height; HitY = height ; Angle = -Angle; }
        }
    }
}

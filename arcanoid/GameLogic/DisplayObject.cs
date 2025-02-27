using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public SolidColorBrush color { get; set; }
        public abstract void Draw(Canvas canvas);
        public virtual void Move(double width, double height)
        {
            double radians = Angle * Math.PI / 180;
            X += Speed * Math.Cos(radians);
            Y += Speed * Math.Sin(radians);
            if (X < 0) { X = 0; Angle = 180 - Angle; }
            if (X > width) { X = width; Angle = 180 - Angle; }
            if (Y < 0) { Y = 0; Angle = -Angle; }
            if (Y > height) { Y = height; Angle = -Angle; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public virtual void Move()
        {
            double radians = Angle * Math.PI / 180;
            X += Speed * Math.Cos(radians);
            Y += Speed * Math.Sin(radians);
        }
    }
}

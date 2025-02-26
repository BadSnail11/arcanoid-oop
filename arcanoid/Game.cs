using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace arcanoid
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
    class Stage
    {
        private List<DisplayObject> objects = new List<DisplayObject>();
        public void AddObject(DisplayObject obj) => objects.Add(obj);
        public void RemoveObject(DisplayObject obj) => objects.Remove(obj);
        public void update()
        {
            foreach (var obj in objects)
                obj.Move();
        }
    }
    class Game
    {
    }
}

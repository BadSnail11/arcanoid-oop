using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arcanoid.GameLogic
{
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
}

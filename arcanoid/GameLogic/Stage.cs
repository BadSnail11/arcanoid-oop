using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace arcanoid.GameLogic
{
    class Stage
    {
        public List<DisplayObject> objects = new List<DisplayObject>();
        public List<DisplayObject> menuObjects = new List<DisplayObject>();
        private Canvas canvas;
        public Stage(Canvas gameCanvas)
        {
            canvas = gameCanvas;
            //DrawBorder(gameCanvas);
        }
        public void AddObject(DisplayObject obj) => objects.Add(obj);
        public void RemoveObject(DisplayObject obj) => objects.Remove(obj);
        public void Update(double canvasWidth, double canvasHeight, bool useAcceleration)
        {
            foreach (var obj in objects)
                obj.Move(canvasWidth, canvasHeight, useAcceleration);
        }
        public void DrawMenu(double canvasWidth, double canvasHeight)
        {
            var obj = menuObjects[0];
            obj.X = canvasWidth / 2 - 75;
            obj.Y = canvasHeight / 2 - 215;
            obj.Draw(canvas);
            for (int i = 1; i < menuObjects.Count; i++)
            {
                obj = menuObjects[i];
                obj.X = canvasWidth / 2 - 50;
                obj.Y = canvasHeight / 2 - 200 + (60 * (i - 1));
                obj.Draw(canvas);
            }
        }
        public void Draw()
        {
            foreach(var obj in objects)
            {
                obj.Draw(canvas);
            }
        }
    }
}

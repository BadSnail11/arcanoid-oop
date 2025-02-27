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
            menuObjects.Add(new RectangleObject(0, 0, 100, 50, Color.FromRgb(50, 50, 50), "Play"));
            menuObjects.Add(new RectangleObject(0, 60, 100, 50, Color.FromRgb(50, 50, 50), "Save"));
            menuObjects.Add(new RectangleObject(0, 120, 100, 50, Color.FromRgb(50, 50, 50), "Load"));
            menuObjects.Add(new RectangleObject(0, 180, 100, 50, Color.FromRgb(50, 50, 50), "Settings"));
            menuObjects.Add(new RectangleObject(0, 240, 100, 50, Color.FromRgb(50, 50, 50), "Exit"));
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
            for (int i = 0; i < menuObjects.Count; i++)
            {
                var obj = menuObjects[i];
                obj.X = canvasWidth / 2 - 50;
                obj.Y = canvasHeight / 2 - 200 + (60 * i);
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

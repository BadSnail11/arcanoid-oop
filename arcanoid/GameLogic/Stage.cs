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
        private Canvas canvas;
        public Stage(Canvas gameCanvas)
        {
            canvas = gameCanvas;
            //DrawBorder(gameCanvas);
        }
        public static void DrawBorder(Canvas canvas)
        {
            Rectangle border = new Rectangle
            {
                Width = canvas.Width - 13,
                Height = canvas.Height - 37,
                Stroke = Brushes.Black,
                StrokeThickness = 5,
                Fill = Brushes.Transparent
            };
            Canvas.SetLeft(border, 0);
            Canvas.SetTop(border, 0);
            canvas.Children.Add(border);
        }
        public void AddObject(DisplayObject obj) => objects.Add(obj);
        public void RemoveObject(DisplayObject obj) => objects.Remove(obj);
        public void Update(double canvasWidth, double canvasHeight, bool useAcceleration)
        {
            foreach (var obj in objects)
                obj.Move(canvasWidth, canvasHeight, useAcceleration);
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

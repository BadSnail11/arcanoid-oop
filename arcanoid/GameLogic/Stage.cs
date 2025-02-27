﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace arcanoid.GameLogic
{
    class Stage
    {
        public List<DisplayObject> objects = new List<DisplayObject>();
        private Canvas canvas;
        public Stage(Canvas gameCanvas)
        {
            canvas = gameCanvas;
        }
        public void AddObject(DisplayObject obj) => objects.Add(obj);
        public void RemoveObject(DisplayObject obj) => objects.Remove(obj);
        public void Update(double canvasWidth, double canvasHeight)
        {
            foreach (var obj in objects)
                obj.Move(canvasWidth, canvasHeight);
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

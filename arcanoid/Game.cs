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
    class Rectangle: DisplayObject
    {
        public Double Width { get; set; }
        public Double Height { get; set; }
        Rectangle(double x, double y, double width, double height, double speed, double angle)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Speed = speed;
            Angle = angle;
            color = new SolidColorBrush(Color.FromRgb((byte)new Random().Next(256), (byte)new Random().Next(256), (byte)new Random().Next(256)));
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
        private Stage stage = new Stage();
        private bool running = true;
        public void Start()
        {
            Task.Run(() =>
            {
                while (running)
                {
                    stage.update();
                    Thread.Sleep(16);
                }
            });
        }
        public void Stop() => running = false;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using static System.Net.Mime.MediaTypeNames;
using System.Windows;

namespace arcanoid.GameLogic
{
    class RectangleObject : DisplayObject
    {
        public Double Width { get; set; }
        public Double Height { get; set; }
        public string Text { get; set; }
        private Rectangle rectangle;
        private TextBlock? textBlock;
        public RectangleObject(double x, double y, double width, double height, Color color, string text)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            this.color = new SolidColorBrush(color);
            Text = text;
            Speed = 0;
            Angle = 0;
            Acceleration = 0;
            AccelAngle = 0;

            rectangle = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = this.color,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            textBlock = new TextBlock
            {
                Text = this.Text,
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                Width = Width
            };
        }
        public RectangleObject(double x, double y, double width, double height, Color color, double speed, double angle, double acceleration, double accelAngle)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Speed = speed;
            Angle = angle;
            Acceleration = acceleration;
            AccelAngle = accelAngle;
            Text = "";
            //color = new SolidColorBrush(Color.FromRgb((byte)new Random().Next(256), (byte)new Random().Next(256), (byte)new Random().Next(256)));
            this.color = new SolidColorBrush(color);

            rectangle = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = this.color,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            textBlock = null;
        }
        public override void Draw(Canvas canvas)
        {
            if (!canvas.Children.Contains(rectangle))
                canvas.Children.Add(rectangle);
            if (!(textBlock is null) && !canvas.Children.Contains(textBlock))
                canvas.Children.Add(textBlock);
            Canvas.SetLeft(rectangle, X);
            Canvas.SetTop(rectangle, Y);
        }
    }
}

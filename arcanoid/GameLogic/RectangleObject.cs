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
using System.Text.Json;
using System.Windows.Media.Media3D;
using System.Text.Json.Serialization;

namespace arcanoid.GameLogic
{
    [Serializable]
    class RectangleObject : DisplayObject
    {
        public Double Width { get; set; }
        public Double Height { get; set; }
        public string Text { get; set; }
        //public double borderSize { get => rectangle.StrokeThickness; set => rectangle.StrokeThickness = value; }
        [JsonIgnore]
        private Rectangle rectangle;
        [JsonIgnore]
        private TextBlock? textBlock;

        private void initRect()
        {
            rectangle = new Rectangle
            {
                Width = this.Width,
                Height = this.Height,
                Fill = this.color,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
        }

        public RectangleObject() { }
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

            initRect();
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
        public override void ChangeBorder(Color color, double borderSize = 1)
        {
            if (rectangle == null)
                initRect();
            rectangle.Stroke = new SolidColorBrush(color);
            rectangle.StrokeThickness = borderSize;
        }
        public override string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
        public override DisplayObject FromJson(string json)
        {
            return JsonSerializer.Deserialize<RectangleObject>(json);
        }
        public override void Draw(Canvas canvas)
        {
            if (rectangle == null)
            {
                initRect();
            }
            if (!canvas.Children.Contains(rectangle))
                canvas.Children.Add(rectangle);
            if (rectangle.Tag == null)
            {
                rectangle.MouseDown += (s, e) => RaiseClickEvent();
                rectangle.Tag = true; // Помечаем, что обработчик уже добавлен
            }
            Canvas.SetLeft(rectangle, X);
            Canvas.SetTop(rectangle, Y);
            if (!(textBlock is null))
            {
                if (!canvas.Children.Contains(textBlock))
                    canvas.Children.Add(textBlock);
                if (textBlock.Tag == null)
                {
                    textBlock.MouseDown += (s, e) => RaiseClickEvent();
                    textBlock.Tag = true; // Помечаем, что обработчик уже добавлен
                }
                Canvas.SetLeft(textBlock!, X);
                Canvas.SetTop(textBlock!, Y + rectangle.Height / 2 - 7);
            }
        }
    }
}

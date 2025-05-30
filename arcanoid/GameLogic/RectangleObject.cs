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

        public bool isPlatform;
        //public double borderSize { get => rectangle.StrokeThickness; set => rectangle.StrokeThickness = value; }
        [JsonIgnore]
        private Rectangle rectangle;
        [JsonIgnore]
        private TextBlock? textBlock;
        [JsonIgnore]
        public bool isPoints = false;

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
            HitBox.Width = Width + 10;
            HitBox.Height = Height + 10;
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
        public RectangleObject(double x, double y, double width, double height, Color color, double speed, double angle, double acceleration, double accelAngle, bool isPlatform = false)
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

            //rectangle = new Rectangle
            //{
            //    Width = width,
            //    Height = height,
            //    Fill = this.color,
            //    Stroke = Brushes.Black,
            //    StrokeThickness = 1
            //};
            initRect();
            textBlock = null;
            this.isPlatform = isPlatform;
        }

        //public override Rect GetHitbox()
        //{
        //    throw new NotImplementedException();
        //}

        public override void ChangeBorder(Color color, double borderSize = 1)
        {
            if (rectangle == null)
                initRect();
            rectangle.Stroke = new SolidColorBrush(color);
            rectangle.StrokeThickness = borderSize;
        }
        public void ChangeTextColor(Color color)
        {
            if (rectangle == null)
                initRect();
            if (textBlock != null)
            {
                textBlock.Foreground = new SolidColorBrush(color);
            }
        }
        public void ChangeText(string text)
        {
            if (textBlock != null)
            {
                Text = text;
                textBlock.Text = text;
            }
        }
        public void ChangeWidth(double delta)
        {
            Width += delta;
            initRect();
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
            Canvas.SetLeft(rectangle, X - Width / 2);
            Canvas.SetTop(rectangle, Y - Height / 2);
            if (!(textBlock is null))
            {
                if (!canvas.Children.Contains(textBlock))
                    canvas.Children.Add(textBlock);
                if (textBlock.Tag == null)
                {
                    textBlock.MouseDown += (s, e) => RaiseClickEvent();
                    textBlock.Tag = true; // Помечаем, что обработчик уже добавлен
                }
                Canvas.SetLeft(textBlock!, X - Width / 2);
                Canvas.SetTop(textBlock!, Y - 7);
            }

            //var hb = GetHitbox();
            //Rectangle hitbox = new Rectangle
            //{
            //    //Width = HitW + 10,
            //    //Height = HitH + 10,
            //    Width = hb.Width,
            //    Height = hb.Height,
            //    Stroke = Brushes.Red,
            //    StrokeDashArray = new DoubleCollection { 2 },
            //    StrokeThickness = 1
            //};
            ////Canvas.SetLeft(hitbox, HitX - 5);
            ////Canvas.SetTop(hitbox, HitY - 5);
            //Canvas.SetLeft(hitbox, hb.X);
            //Canvas.SetTop(hitbox, hb.Y);
            //canvas.Children.Add(hitbox);
        }
    }
}

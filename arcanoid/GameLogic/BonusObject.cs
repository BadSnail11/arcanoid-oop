using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Shapes;

namespace arcanoid.GameLogic
{
    enum Types
    {
        pBalls,
        pWidth,
        pTry,
        pSpeed,
        mBSpeed,
        mBalls,
        mWidth,
        mTry,
        mSpeed,
        pBSpeed,
    }
    class BonusObject : RectangleObject
    {
        public Types Type;
        [JsonIgnore]
        private Image image;
        public BonusObject(double x, double y, Types type) : base(x, y, 64d, 64d, Color.FromArgb(0, 0, 0, 0), 3, 90, 0, 0)
        {
            Type = type;
            image = new Image();
            image.Width = 64;
            image.Height = 64;
            //BitmapImage bitmap = new BitmapImage();
            //bitmap.BeginInit();
            //bitmap.UriSource = new Uri($"images/{Type.ToString()}.png");
            //bitmap.EndInit();
            //image.Source = bitmap;
            string pathImage = $"pack://application:,,,/arcanoid;component/images/{type}.png";
            BitmapImage bitmap = new BitmapImage(new Uri(pathImage, UriKind.Absolute));
            image.Source = bitmap;
        }

        public override void Draw(Canvas canvas)
        {
            Canvas.SetLeft(image, X - Width / 2);
            Canvas.SetTop(image, Y - Width / 2);
            canvas.Children.Add(image);
            //var hb = GetHitbox();
            //Rectangle hitbox = new Rectangle
            //{
            //    Width = hb.Width,
            //    Height = hb.Height,
            //    Stroke = Brushes.Red,
            //    StrokeDashArray = new DoubleCollection { 2 },
            //    StrokeThickness = 1
            //};
            //Canvas.SetLeft(hitbox, hb.X);
            //Canvas.SetTop(hitbox, hb.Y);
            //canvas.Children.Add(hitbox);
        }
    }
}

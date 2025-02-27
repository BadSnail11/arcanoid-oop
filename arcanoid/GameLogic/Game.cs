using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace arcanoid.GameLogic
{
    class Game
    {
        private Stage stage;
        private bool running = true;
        private DispatcherTimer timer;
        private Random random = new Random();
        private double canvasWidth;
        private double canvasHeight;
        private Canvas gameCanvas;
        private Window mainWindow;

        public Game(Window window, Canvas gameCanvas)
        {
            this.gameCanvas = gameCanvas;
            this.mainWindow = window;
            stage = new Stage(gameCanvas);
            //canvasWidth = gameCanvas.Width;
            //canvasHeight = gameCanvas.Height;
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            timer.Tick += (s, e) => { stage.Update(gameCanvas.Width, gameCanvas.Height); stage.Draw(); };
            InitializeObjects();

            mainWindow.SizeChanged += (s, e) =>
            {
                gameCanvas.Width = mainWindow.Width;
                gameCanvas.Height = mainWindow.Height;
            };
        }
        private void InitializeObjects()
        {
            for (int i = 0; i < 10; i++)
            {
                stage.AddObject(new RectangleObject(random.Next(50, 750), random.Next(50, 550), 40, 20, random.Next(2, 6), random.Next(0, 360)));
                stage.AddObject(new TriangleObject(random.Next(50, 750), random.Next(50, 550), 30, random.Next(2, 6), random.Next(0, 360)));
                stage.AddObject(new TrapezoidObject(random.Next(50, 750), random.Next(50, 550), 50, 30, random.Next(2, 6), random.Next(0, 360)));
                stage.AddObject(new CircleObject(random.Next(50, 750), random.Next(50, 550), 15, random.Next(2, 6), random.Next(0, 360)));
            }
        }

        public void Start() => timer.Start();
        public void Stop() => timer.Stop();
    }
}

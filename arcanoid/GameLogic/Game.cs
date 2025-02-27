using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Shapes;

namespace arcanoid.GameLogic
{
    class Game
    {
        private Stage stage;
        private bool isRunning = false;
        private bool isPaused = true;
        private DispatcherTimer renderTimer;
        private Thread physicsThread;
        private Random random = new Random();
        private double canvasWidth;
        private double canvasHeight;
        private Canvas gameCanvas;
        private Window mainWindow;
        private bool isFullscreen = false;

        public Game(Window window, Canvas gameCanvas)
        {
            this.gameCanvas = gameCanvas;
            this.mainWindow = window;
            stage = new Stage(gameCanvas);
            //canvasWidth = gameCanvas.Width;
            //canvasHeight = gameCanvas.Height;
            renderTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(0.1) };
            renderTimer.Tick += (s, e) => { stage.Draw(); };
            InitializeObjects();


            mainWindow.SizeChanged += (s, e) =>
            {
                gameCanvas.Width = mainWindow.Width;
                gameCanvas.Height = mainWindow.Height;
                EnsureObjectsWithinBounds();
            };

            mainWindow.KeyDown += (s, e) =>
            {
                if (e.Key == Key.F)
                {
                    ToggleFullscreen();
                }
                else if (e.Key == Key.P)
                {
                    TogglePause();
                }
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
        private void ToggleFullscreen()
        {
            if (isFullscreen)
            {
                mainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                mainWindow.WindowState = WindowState.Normal;
            }
            else
            {
                mainWindow.WindowStyle = WindowStyle.None;
                mainWindow.WindowState = WindowState.Maximized;
            }
            isFullscreen = !isFullscreen;
        }
        private void TogglePause()
        {
            isPaused = !isPaused;
        }
        private void EnsureObjectsWithinBounds()
        {
            foreach (var obj in stage.objects)
            {
                if (obj.X < 0) obj.X = 0;
                if (obj.X > gameCanvas.Width) obj.X = gameCanvas.Width;
                if (obj.Y < 0) obj.Y = 0;
                if (obj.Y > gameCanvas.Height) obj.Y = gameCanvas.Height;
            }
        }
        private void PhysicsLoop()
        {
            while (isRunning)
            {
                if (!isPaused)
                {
                    double width = 0, height = 0;

                    gameCanvas.Dispatcher.Invoke(() =>
                    {
                        width = gameCanvas.Width;
                        height = gameCanvas.Height;
                    });

                    stage.Update(width, height);
                }
                Thread.Sleep(16); // 60 FPS
            }
        }
        public void Start()
        {
            isRunning = true;
            renderTimer.Start();
            physicsThread = new Thread(PhysicsLoop) { IsBackground = true };
            physicsThread.Start();
        }
        public void Stop()
        {
            isRunning = false;
            renderTimer.Stop();
            physicsThread.Join();
        }
    }
}

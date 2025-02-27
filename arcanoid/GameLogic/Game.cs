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
        private bool useAcceleration = false;
        private bool isMenu = false;

        public Game(Window window, Canvas gameCanvas)
        {
            this.gameCanvas = gameCanvas;
            this.mainWindow = window;
            stage = new Stage(this.gameCanvas);
            renderTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(0.1) };
            renderTimer.Tick += (s, e) => {
                this.gameCanvas.Children.Clear();
                stage.Draw();
                if (isMenu) stage.DrawMenu(gameCanvas.Width, gameCanvas.Height);
            };
            InitializeObjects();
            ToggleFullscreen();


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
                else if (e.Key == Key.A)
                {
                    useAcceleration = !useAcceleration;
                }
                else if (e.Key == Key.Escape)
                {
                    ToggleMenu();
                }
            };
        }
        private void InitializeObjects()
        {
            stage.menuObjects.Add(new RectangleObject(0, 0, 150, 320, Color.FromRgb(100, 100, 100), ""));
            var playRect = new RectangleObject(0, 0, 100, 50, Color.FromRgb(50, 50, 50), "Play");
            playRect.OnClick += obj => ToggleMenu();
            stage.menuObjects.Add(playRect);
            var saveRect = new RectangleObject(0, 60, 100, 50, Color.FromRgb(50, 50, 50), "Save");
            stage.menuObjects.Add(saveRect);
            var loadRect = new RectangleObject(0, 120, 100, 50, Color.FromRgb(50, 50, 50), "Load");
            stage.menuObjects.Add(loadRect);
            var settingsRect = new RectangleObject(0, 180, 100, 50, Color.FromRgb(50, 50, 50), "Settings");
            stage.menuObjects.Add(settingsRect);
            var exitRect = new RectangleObject(0, 240, 100, 50, Color.FromRgb(50, 50, 50), "Exit");
            exitRect.OnClick += obj => ToggleExit();
            stage.menuObjects.Add(exitRect);
            for (int i = 0; i < 10; i++)
            {
                stage.AddObject(new RectangleObject(
                    x: random.Next(50, 750),
                    y: random.Next(50, 550),
                    width: random.Next(20, 80),
                    height: random.Next(20, 80),
                    color: Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)),
                    speed: random.Next(2, 4),
                    angle: random.Next(0, 360),
                    acceleration: random.Next(2, 6),
                    accelAngle: random.Next(0, 360)));
                stage.AddObject(new TriangleObject(
                    x: random.Next(50, 750),
                    y: random.Next(50, 550),
                    size: random.Next(20, 60),
                    color: Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)),
                    speed: random.Next(2, 4),
                    angle: random.Next(0, 360),
                    acceleration: random.Next(2, 6),
                    accelAngle: random.Next(0, 360)));
                stage.AddObject(new TrapezoidObject(
                    x: random.Next(50, 750),
                    y: random.Next(50, 550),
                    width: random.Next(20, 80),
                    height: random.Next(20, 80),
                    color: Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)),
                    speed: random.Next(2, 4),
                    angle: random.Next(0, 360),
                    acceleration: random.Next(2, 6),
                    accelAngle: random.Next(0, 360)));
                stage.AddObject(new CircleObject(
                    x: random.Next(50, 750),
                    y: random.Next(50, 550),
                    radius: random.Next(15, 40),
                    color: Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)),
                    speed: random.Next(2, 4),
                    angle: random.Next(0, 360),
                    acceleration: random.Next(2, 6),
                    accelAngle: random.Next(0, 360)));
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
        private void ToggleMenu()
        {
            isMenu = !isMenu;
        }
        private void TogglePause()
        {
            isPaused = !isPaused;
        }
        private void ToggleExit()
        {
            mainWindow.Close();
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
                if (!isPaused && !isMenu)
                {
                    double width = 0, height = 0;

                    gameCanvas.Dispatcher.Invoke(() =>
                    {
                        width = gameCanvas.Width;
                        height = gameCanvas.Height;
                    });

                    stage.Update(width, height, useAcceleration);
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

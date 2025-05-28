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
using System.Windows.Media.Media3D;
using System.Numerics;
using System.Reflection.Metadata;

namespace arcanoid.GameLogic
{
    class Game
    {
        private Stage stage;
        private bool isRunning = false;
        private bool isPaused = true;
        private DispatcherTimer renderTimer;
        private Thread physicsThread;
        private Thread collisionsThread;
        private Random random = new Random();
        private double canvasWidth;
        private double canvasHeight;
        private Canvas gameCanvas;
        private Window mainWindow;
        private bool isFullscreen = false;
        private bool useAcceleration = false;
        private bool isMenu = false;
        private bool isSettings = false;

        private List<Tuple<DisplayObject, DisplayObject>> collisionQueue = new();

        public Game(Window window, Canvas gameCanvas)
        {
            this.gameCanvas = gameCanvas;
            this.mainWindow = window;
            stage = new Stage(this.gameCanvas);
            renderTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(0.1) };
            renderTimer.Tick += (s, e) => {
                this.gameCanvas.Children.Clear();
                stage.Draw();
                stage.DrawBorder(gameCanvas.Width, (isFullscreen) ? gameCanvas.Height : gameCanvas.Height - 23);
                if (isMenu && !isSettings) stage.DrawMenu(gameCanvas.Width, gameCanvas.Height);
                if (isSettings) stage.DrawSettings(gameCanvas.Width, gameCanvas.Height);
            };
            ToggleFullscreen();
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
            // меню
            stage.menuObjects.Add(new RectangleObject(0, 0, 150, 320, Color.FromRgb(100, 100, 100), ""));
            var playRect = new RectangleObject(0, 0, 100, 50, Color.FromRgb(50, 50, 50), "Play");
            playRect.OnClick += obj => ToggleMenu();
            stage.menuObjects.Add(playRect);
            var saveRect = new RectangleObject(0, 60, 100, 50, Color.FromRgb(50, 50, 50), "Save");
            saveRect.OnClick += obj => stage.SaveObjectsToFile();
            stage.menuObjects.Add(saveRect);
            var loadRect = new RectangleObject(0, 120, 100, 50, Color.FromRgb(50, 50, 50), "Load");
            loadRect.OnClick += obj => stage.LoadObjectsFromFile();
            stage.menuObjects.Add(loadRect);
            var settingsRect = new RectangleObject(0, 180, 100, 50, Color.FromRgb(50, 50, 50), "Settings");
            settingsRect.OnClick += obj => ToggleSetings();
            stage.menuObjects.Add(settingsRect);
            var exitRect = new RectangleObject(0, 240, 100, 50, Color.FromRgb(50, 50, 50), "Exit");
            exitRect.OnClick += obj => ToggleExit();
            stage.menuObjects.Add(exitRect);

            // настройки
            stage.settingsObjects.Add(new RectangleObject(0, 0, 150, 320, Color.FromRgb(100, 100, 100), ""));
            var decsriptionRect = new RectangleObject(0, 60, 100, 50, Color.FromRgb(50, 50, 50), "Choose back");
            stage.settingsObjects.Add(decsriptionRect);
            var color1 = new RectangleObject(0, 0, 100, 50, Color.FromRgb(255, 253, 191), "");
            color1.OnClick += obj => ChangeBackground(Color.FromRgb(255, 253, 191));
            stage.settingsObjects.Add(color1);
            var color2 = new RectangleObject(0, 0, 100, 50, Color.FromRgb(176, 210, 255), "");
            color2.OnClick += obj => ChangeBackground(Color.FromRgb(176, 210, 255));
            stage.settingsObjects.Add(color2);
            var color3 = new RectangleObject(0, 0, 100, 50, Color.FromRgb(233, 181, 255), "");
            color3.OnClick += obj => ChangeBackground(Color.FromRgb(233, 181, 255));
            stage.settingsObjects.Add(color3);
            var backRect = new RectangleObject(0, 60, 100, 50, Color.FromRgb(50, 50, 50), "Back");
            backRect.OnClick += obj => ToggleSetings();
            stage.settingsObjects.Add(backRect);

            // игровые объекты
            for (int i = 0; i < 20; i++)
            {
                //stage.AddObject(new RectangleObject(
                //    x: random.Next(50, 1800),
                //    y: random.Next(50, 1000),
                //    width: random.Next(20, 80),
                //    height: random.Next(20, 80),
                //    color: Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)),
                //    speed: random.Next(10, 15),
                //    angle: random.Next(0, 360),
                //    acceleration: random.Next(6, 10),
                //    accelAngle: random.Next(0, 360)));
                //stage.AddObject(new TriangleObject(
                //    x: random.Next(50, 1800),
                //    y: random.Next(50, 1000),
                //    size: random.Next(20, 60),
                //    color: Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)),
                //    speed: random.Next(10, 15),
                //    angle: random.Next(0, 360),
                //    acceleration: random.Next(6, 10),
                //    accelAngle: random.Next(0, 360)));
                //stage.AddObject(new TrapezoidObject(
                //    x: random.Next(50, 1800),
                //    y: random.Next(50, 1000),
                //    width: random.Next(20, 80),
                //    height: random.Next(20, 80),
                //    color: Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)),
                //    speed: random.Next(10, 15),
                //    angle: random.Next(0, 360),
                //    acceleration: random.Next(6, 10),
                //    accelAngle: random.Next(0, 360)));
                var circle = new CircleObject(
                    x: random.Next(50, 1800),
                    y: random.Next(50, 1000),
                    radius: random.Next(35, 55),
                    color: Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)),
                    speed: random.Next(2, 4),
                    angle: random.Next(0, 360),
                    acceleration: random.Next(6, 10),
                    accelAngle: random.Next(0, 360));
                while (!CanPlace(circle))
                {
                    circle = new CircleObject(
                    x: random.Next(50, 1800),
                    y: random.Next(50, 1000),
                    radius: random.Next(35, 55),
                    color: Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)),
                    speed: random.Next(2, 4),
                    angle: random.Next(0, 360),
                    acceleration: random.Next(6, 10),
                    accelAngle: random.Next(0, 360));
                }
                //circle.ChangeBorder(Color.FromRgb(0, 0, 0), 2);
                stage.AddObject(circle);
            }
        }

        private bool CanPlace(DisplayObject obj)
        {
            foreach (var el in stage.objects)
            {
                if (IfCollides(obj, el)) return false;
                var hitbox = obj.GetHitbox();
                if (hitbox.Left < 0) return false;
                if (hitbox.Right > 1600) return false;
                if (hitbox.Top < 0) return false;
                if (hitbox.Bottom > 900) return false;
            }
            return true;
        }

        private bool IfCollides(DisplayObject obj1, DisplayObject obj2)
        {
            return obj1.GetHitbox().IntersectsWith(obj2.GetHitbox());
        }

        // логика кнопок
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
            if (isSettings)
                isSettings = !isSettings;
        }
        private void ToggleSetings()
        {
            isSettings = !isSettings;
        }
        private void TogglePause()
        {
            isPaused = !isPaused;
        }
        private void ToggleExit()
        {
            mainWindow.Close();
        }
        private void ChangeBackground(Color color)
        {
            gameCanvas.Background = new SolidColorBrush(color);
        }

        // логика объектов
        private void EnsureObjectsWithinBounds()
        {
            foreach (var obj in stage.objects)
            {
                if (obj.X < 0)
                {
                    obj.X = 0;
                    //obj.HitX = 0;
                }
                if (obj.X > gameCanvas.Width)
                {
                    obj.X = gameCanvas.Width;
                    //obj.HitX = gameCanvas.Width;
                }
                if (obj.Y < 0)
                {
                    obj.Y = 0;
                    //obj.HitY = 0;
                }
                if (obj.Y > gameCanvas.Height)
                {
                    obj.Y = gameCanvas.Height;
                    //obj.HitY = gameCanvas.Height;   
                }
            }
        }

        public void CheckCollisions(List<DisplayObject> objects)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                for (int j = i + 1; j < objects.Count; j++)
                {
                    if (objects[i].GetHitbox().IntersectsWith(objects[j].GetHitbox()))
                    {
                        //collisionQueue.Add(objects[i]);
                        //collisionQueue.Add(objects[j]);
                        var pair = new Tuple<DisplayObject, DisplayObject>(objects[i], objects[j]);
                        collisionQueue.Add(pair);
                    }
                }
            }
        }

        public void ProcessCollisions()
        {
            while (isRunning)
            {
                while (collisionQueue.Count > 0)
                {
                    if (collisionQueue.Count == 0) continue;

                    var pair = collisionQueue.FirstOrDefault();
                    try
                    {
                        var obj1 = pair.Item1;
                        var obj2 = pair.Item2;

                        _ = Task.Run(() => HandleCollision(obj1, obj2));
                    }
                    catch (Exception ex)
                    {
                    }

                    collisionQueue.Remove(pair);
                }
            }
        }

        private async Task HandleCollision(DisplayObject object1, DisplayObject object2)
        {
            var obj1 = object1 as CircleObject;
            var obj2 = object2 as CircleObject;

            if (obj1 != null && obj2 != null)
            {
                double dist;
                double minDist;

                var dx = obj1.X - obj2.X;
                var dy = obj1.Y - obj2.Y;
                minDist = obj1.Radius + obj2.Radius;
                dist = Math.Sqrt(dx * dx + dy * dy);
                if (dist >= minDist) return;

                Vector2 normal = new Vector2((float)dx, (float)dy);
                normal = Vector2.Normalize(normal);

                obj1.Reflect(normal);
                obj2.Reflect(-normal);

                minDist = obj1.Radius + obj2.Radius + 5;
                dist = Math.Sqrt(dx * dx + dy * dy);
                var overlap = minDist - dist;
                Vector2 correction = normal * ((float)overlap * 0.1f);
                obj1.X += correction.X;
                obj1.Y += correction.Y;
                obj2.X -= correction.X;
                obj2.Y -= correction.Y;
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

                    if (!isFullscreen)
                        stage.Update(width - 2, height-25, useAcceleration);
                    else
                        stage.Update(width - 2, height, useAcceleration);

                    CheckCollisions(stage.objects);
                }
                Thread.Sleep(5);
            }
        }
        public void Start()
        {
            isRunning = true;
            renderTimer.Start();
            physicsThread = new Thread(PhysicsLoop) { IsBackground = true };
            collisionsThread = new Thread(ProcessCollisions) { IsBackground = true };
            physicsThread.Start();
            collisionsThread.Start();
        }
        public void Stop()
        {
            isRunning = false;
            renderTimer.Stop();
            physicsThread.Join();
            collisionsThread.Join();
        }
    }
}

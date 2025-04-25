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

        private List<DisplayObject> collisionQueue = new();

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
                stage.AddObject(new CircleObject(
                    x: random.Next(50, 1800),
                    y: random.Next(50, 1000),
                    radius: random.Next(15, 40),
                    color: Color.FromRgb((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)),
                    speed: random.Next(2, 6),
                    angle: random.Next(0, 360),
                    acceleration: random.Next(6, 10),
                    accelAngle: random.Next(0, 360)));
            }
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
                    obj.HitX = 0;
                }
                if (obj.X > gameCanvas.Width)
                {
                    obj.X = gameCanvas.Width;
                    obj.HitX = gameCanvas.Width;
                }
                if (obj.Y < 0)
                {
                    obj.Y = 0;
                    obj.HitY = 0;
                }
                if (obj.Y > gameCanvas.Height)
                {
                    obj.Y = gameCanvas.Height;
                    obj.HitY = gameCanvas.Height;   
                }
            }
        }

        public void CheckCollisions(List<DisplayObject> objects)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                for (int j = i + 1; j < objects.Count; j++)
                {
                    if (objects[i].HitBox.IntersectsWith(objects[j].HitBox))
                    {
                        collisionQueue.Add(objects[i]);
                        collisionQueue.Add(objects[j]);
                    }
                }
            }
        }

        public void ProcessCollisions()
        {
            while (isRunning)
            {
                for (int i = 0; i < collisionQueue.Count; i += 2)
                {
                    var obj1 = collisionQueue[i];
                    var obj2 = collisionQueue[i + 1];
                    if (obj1 is CircleObject circle1 && obj2 is CircleObject circle2)
                    {
                        double dx = circle2.X - circle1.X;
                        double dy = circle2.Y - circle1.Y;
                        double distance = Math.Sqrt(dx * dx + dy * dy);
                        double minDistance = circle1.Radius + circle2.Radius;

                        if (distance < minDistance && distance >= 0)
                        {
                            // Смещение чтобы избежать повторного столкновения
                            double overlap = minDistance - distance;
                            double shiftX = overlap * dx / distance / 2;
                            double shiftY = overlap * dy / distance / 2;

                            circle1.X -= shiftX;
                            circle1.HitX -= shiftX;
                            circle1.Y -= shiftY;
                            circle1.HitY -= shiftY;
                            circle2.X += shiftX;
                            circle2.HitX += shiftX;
                            circle2.Y += shiftY;
                            circle2.HitY += shiftY;

                            // Обмен скоростями
                            double angle1 = circle1.Angle * Math.PI / 180;
                            double angle2 = circle2.Angle * Math.PI / 180;

                            double nx = dx / distance;
                            double ny = dy / distance;
                            double tx = -ny;
                            double ty = nx;

                            double vx1 = circle1.Speed * Math.Cos(angle1);
                            double vy1 = circle1.Speed * Math.Sin(angle1);
                            double vx2 = circle2.Speed * Math.Cos(angle2);
                            double vy2 = circle2.Speed * Math.Sin(angle2);

                            double dpTan1 = vx1 * tx + vy1 * ty;
                            double dpTan2 = vx2 * tx + vy2 * ty;

                            double dpNorm1 = vx1 * nx + vy1 * ny;
                            double dpNorm2 = vx2 * nx + vy2 * ny;

                            double m1 = dpNorm2;
                            double m2 = dpNorm1;

                            vx1 = tx * dpTan1 + nx * m1;
                            vy1 = ty * dpTan1 + ny * m1;
                            vx2 = tx * dpTan2 + nx * m2;
                            vy2 = ty * dpTan2 + ny * m2;

                            //circle1.Speed = Math.Sqrt(vx1 * vx1 + vy1 * vy1);
                            circle1.Angle = Math.Atan2(vy1, vx1) * 180 / Math.PI;

                            //circle2.Speed = Math.Sqrt(vx2 * vx2 + vy2 * vy2);
                            circle2.Angle = Math.Atan2(vy2, vx2) * 180 / Math.PI;

                            //double width = 0, height = 0;

                            //gameCanvas.Dispatcher.Invoke(() =>
                            //{
                            //    width = gameCanvas.Width;
                            //    height = gameCanvas.Height;
                            //});

                            //circle1.Move(width, height, useAcceleration);
                            //circle1.Move(width, height, useAcceleration);
                        }
                    }
                    else
                    {
                        double angle1 = obj1.Angle * Math.PI / 180;
                        double angle2 = obj2.Angle * Math.PI / 180;

                        double vx1 = obj1.Speed * Math.Cos(angle1);
                        double vy1 = obj1.Speed * Math.Sin(angle1);
                        double vx2 = obj2.Speed * Math.Cos(angle2);
                        double vy2 = obj2.Speed * Math.Sin(angle2);

                        //obj1.Speed = Math.Sqrt(vx2 * vx2 + vy2 * vy2);
                        obj1.Angle = Math.Atan2(vy2, vx2) * 180 / Math.PI;

                        //obj2.Speed = Math.Sqrt(vx1 * vx1 + vy1 * vy1);
                        obj2.Angle = Math.Atan2(vy1, vx1) * 180 / Math.PI;


                        //stage.Update(width, height, useAcceleration);
                    }
                }
                collisionQueue.Clear();
                Thread.Sleep(16);
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

                    CheckCollisions(stage.objects);
                }
                Thread.Sleep(16); // 60 FPS
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

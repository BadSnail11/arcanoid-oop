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
                for (int i = 0; i < stage.objects.Count; i++)
                {
                    for (int j = 0; j < stage.objects.Count; j++)
                    {
                        if (i == j) continue;
                        var obj1 = stage.objects[i] as CircleObject;
                        var obj2 = stage.objects[j] as CircleObject;

                        if (obj1 != null && obj2 != null)
                        {
                            // Исходные положения (t=0)
                            double x11 = obj1.X;
                            double y11 = obj1.Y;
                            double x21 = obj2.X;
                            double y21 = obj2.Y;

                            // Конечные положения (t=1)
                            double x12 = x11 + obj1.Speed * Math.Cos(obj1.Angle * Math.PI / 180);
                            double y12 = y11 + obj1.Speed * Math.Sin(obj1.Angle * Math.PI / 180);
                            double x22 = x21 + obj2.Speed * Math.Cos(obj2.Angle * Math.PI / 180);
                            double y22 = y21 + obj2.Speed * Math.Sin(obj2.Angle * Math.PI / 180);

                            // Вычисляем коэффициенты квадратного уравнения
                            double a = x11 * x11 + x12 * x12 + y11 * y11 + y12 * y12 +
                                      x21 * x21 + x22 * x22 + y21 * y21 + y22 * y22 +
                                      2 * (-x11 * x12 - x21 * x22 - y11 * y12 - y21 * y22 -
                                          x11 * x21 - y11 * y21 + x11 * x22 + y11 * y22 +
                                          x12 * x21 + y12 * y21 - x12 * x22 - y12 * y22);

                            double b = 2 * (-x11 * x11 - x21 * x21 - y11 * y11 - y21 * y21 +
                                          x11 * x12 + y11 * y12 + x21 * x22 + y21 * y22 -
                                          x11 * x22 - y11 * y22 - x12 * x21 - y12 * y21 +
                                          2 * x11 * x21 + 2 * y11 * y21);

                            double c = x11 * x11 - 2 * x11 * x21 + x21 * x21 +
                                      y11 * y11 - 2 * y11 * y21 + y21 * y21 -
                                      (obj1.Radius + obj2.Radius) * (obj1.Radius + obj2.Radius);

                            // Решаем квадратное уравнение
                            double discriminant = b * b - 4 * a * c;

                            if (discriminant >= 0 && a != 0)
                            {
                                double sqrtDiscriminant = Math.Sqrt(discriminant);
                                double t1 = (-b + sqrtDiscriminant) / (2 * a);
                                double t2 = (-b - sqrtDiscriminant) / (2 * a);

                                // Нас интересуют только решения в интервале [0, 1]
                                List<double> validTs = new List<double>();
                                if (t1 >= 0 && t1 <= 1) validTs.Add(t1);
                                if (t2 >= 0 && t2 <= 1) validTs.Add(t2);

                                if (validTs.Count > 0)
                                {
                                    // Берем наименьшее t (первое столкновение)
                                    double t = validTs.Min();

                                    // Вычисляем положения в момент столкновения
                                    double x1 = x11 + (x12 - x11) * t;
                                    double y1 = y11 + (y12 - y11) * t;
                                    double x2 = x21 + (x22 - x21) * t;
                                    double y2 = y21 + (y22 - y21) * t;

                                    // Вектор между центрами в момент столкновения
                                    double nx = x2 - x1;
                                    double ny = y2 - y1;
                                    double distance = Math.Sqrt(nx * nx + ny * ny);
                                    nx /= distance;
                                    ny /= distance;

                                    // Тангенциальные компоненты
                                    double tx = -ny;
                                    double ty = nx;

                                    // Разлагаем скорости на нормальные и тангенциальные компоненты
                                    double v1x = obj1.Speed * Math.Cos(obj1.Angle * Math.PI / 180);
                                    double v1y = obj1.Speed * Math.Sin(obj1.Angle * Math.PI / 180);
                                    double v2x = obj2.Speed * Math.Cos(obj2.Angle * Math.PI / 180);
                                    double v2y = obj2.Speed * Math.Sin(obj2.Angle * Math.PI / 180);

                                    // Проекции на нормаль и тангенс
                                    double v1n = v1x * nx + v1y * ny;
                                    double v1t = v1x * tx + v1y * ty;
                                    double v2n = v2x * nx + v2y * ny;
                                    double v2t = v2x * tx + v2y * ty;

                                    // Обмен нормальными компонентами скоростей
                                    double v1nAfter = v2n;
                                    double v2nAfter = v1n;

                                    // Собираем новые скорости
                                    double v1xAfter = v1nAfter * nx + v1t * tx;
                                    double v1yAfter = v1nAfter * ny + v1t * ty;
                                    double v2xAfter = v2nAfter * nx + v2t * tx;
                                    double v2yAfter = v2nAfter * ny + v2t * ty;

                                    // Обновляем скорости и углы объектов
                                    //obj1.Speed = Math.Sqrt(v1xAfter * v1xAfter + v1yAfter * v1yAfter);
                                    obj1.Angle = Math.Atan2(v1yAfter, v1xAfter) * 180 / Math.PI;
                                    //obj2.Speed = Math.Sqrt(v2xAfter * v2xAfter + v2yAfter * v2yAfter);
                                    obj2.Angle = Math.Atan2(v2yAfter, v2xAfter) * 180 / Math.PI;

                                    // Корректируем положения, чтобы избежать залипания
                                    double overlap = (obj1.Radius + obj2.Radius) - distance;
                                    double shiftX = overlap * nx / 2;
                                    double shiftY = overlap * ny / 2;

                                    obj1.X = x1 - shiftX;
                                    obj1.Y = y1 - shiftY;
                                    obj2.X = x2 + shiftX;
                                    obj2.Y = y2 + shiftY;
                                }
                            }
                        }
                    }
                }
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

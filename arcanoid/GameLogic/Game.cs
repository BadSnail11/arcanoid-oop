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
using System.Windows.Documents;

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

        private int platformSpeed = 15;

        private List<Tuple<DisplayObject, DisplayObject>> collisionQueue = new();

        public Game(Window window, Canvas gameCanvas)
        {
            this.gameCanvas = gameCanvas;
            this.mainWindow = window;
            stage = new Stage(this.gameCanvas);
            stage.gameStat = new GameStat(1, 0, 3);
            renderTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(0.1) };
            renderTimer.Tick += (s, e) => {
                this.gameCanvas.Children.Clear();
                stage.Draw();
                stage.DrawBorder(gameCanvas.Width, (isFullscreen) ? gameCanvas.Height : gameCanvas.Height - 23);
                if (isMenu && !isSettings) stage.DrawMenu(gameCanvas.Width, gameCanvas.Height);
                if (isSettings) stage.DrawSettings(gameCanvas.Width, gameCanvas.Height);
                stage.DrawStat(gameCanvas.Width, gameCanvas.Height);
                CheckLevelStatus();
            };
            ToggleFullscreen();
            InitializeServiceObjects();
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
                else if (e.Key == Key.Space)
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
                else if (e.Key == Key.Right)
                {
                    ToggleRightButton();
                }
                else if (e.Key == Key.Left)
                {
                    ToggleLeftButton();
                }
            };

            
        }

        private void InitializeServiceObjects()
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
        }
        private void InitializeObjects()
        {
            

            //игровые объекты
            for (int i = 0; i < stage.gameStat.Level * 7; i++)
            {
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
            // главный шарик
            //stage.AddObject(new CircleObject(
            //        x: random.Next(50, 1800),
            //        y: random.Next(50, 1000),
            //        radius: 20,
            //        color: Color.FromRgb((byte)255, (byte)255, (byte)255),
            //        speed: random.Next(6, 8),
            //        angle: random.Next(0, 360),
            //        acceleration: random.Next(6, 10),
            //        accelAngle: random.Next(0, 360),
            //        isMain: true));
            // платформа
            stage.AddObject(new RectangleObject(
                x: 775,
                y: 875,
                width: 200,
                height: 50,
                color: Color.FromRgb(0, 0, 0),
                speed: 0.0,
                angle: 0.0,
                acceleration: 0.0,
                accelAngle: 0.0,
                isPlatform: true));
            SpawnMainBall();
        }

        private void SpawnMainBall(double x = 0, double y = 0)
        {
            if (x == 0d && y == 0d)
            {
                foreach (var obj in stage.objects)
                {
                    if (obj is RectangleObject rect && rect.isPlatform)
                    {
                        x = rect.X;
                        y = rect.Y - 50;
                        break;
                    }
                }
            }
            stage.AddObject(new CircleObject(
                    x: x,
                    y: y,
                    radius: 20,
                    color: Color.FromRgb((byte)255, (byte)255, (byte)255),
                    speed: random.Next(6, 8),
                    angle: random.Next(225, 315),
                    acceleration: random.Next(6, 10),
                    accelAngle: random.Next(0, 360),
                    isMain: true));
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

        private void ToggleRightButton()
        {
            if (isPaused)
                return;
            foreach (var obj in stage.objects)
            {
                if (obj is RectangleObject rect && rect.isPlatform)
                    rect.X += platformSpeed;
            }
        }

        private void ToggleLeftButton()
        {
            if (isPaused)
                return;
            foreach (var obj in stage.objects)
            {
                if (obj is RectangleObject rect && rect.isPlatform)
                    rect.X -= platformSpeed;
            }
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
                    if (pair == null)
                    {
                        collisionQueue.Remove(pair);
                        continue;
                    }
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
            //var obj1 = object1 as CircleObject;
            //var obj2 = object2 as CircleObject;

            if ((object1 is RectangleObject && object2 is BonusObject) || (object2 is RectangleObject && object1 is BonusObject))
            {
                RectangleObject rect;
                BonusObject bonus;
                if (object1 is RectangleObject)
                {
                    rect = object1 as RectangleObject;
                    bonus = object2 as BonusObject;
                } else
                {
                    rect = object2 as RectangleObject;
                    bonus = object1 as BonusObject;
                }
                if (!rect.isPlatform)
                    return;
                gameCanvas.Dispatcher.Invoke(() => ApplyBonus(bonus));
                bonus.isDeleted = true;
            }

            if ((object1 is CircleObject && object2 is RectangleObject) || (object2 is CircleObject && object1 is RectangleObject))
            {
                CircleObject circle;
                RectangleObject rect;
                if (object1 is CircleObject)
                {
                    circle = object1 as CircleObject;
                    rect = object2 as RectangleObject;
                }
                else
                {
                    circle = object2 as CircleObject;
                    rect = object1 as RectangleObject;
                }
                if (rect.isPoints || rect is BonusObject)
                    return;
                    // Находим ближайшую точку на прямоугольнике к центру круга
                float closestX = (float)Math.Clamp(circle.X, rect.X - rect.Width / 2, rect.X + rect.Width / 2);
                float closestY = (float)Math.Clamp(circle.Y, rect.Y - rect.Height / 2, rect.Y + rect.Height / 2);

                // Вычисляем расстояние от центра круга до ближайшей точки
                float dx = (float)(circle.X - closestX);
                float dy = (float)(circle.Y - closestY);
                float distanceSquared = dx * dx + dy * dy;

                // Проверяем коллизию
                if (distanceSquared > circle.Radius * circle.Radius) return;

                // Нормализуем вектор столкновения
                float distance = (float)Math.Sqrt(distanceSquared);
                Vector2 normal = distance > 0
                    ? new Vector2(dx / distance, dy / distance)
                    : new Vector2(0, -1); // Если расстояние 0 (центр внутри), отталкиваем вверх

                // Отражение круга
                circle.Reflect(normal);

                // Коррекция позиции для избежания залипания
                float overlap = (float)(circle.Radius - distance);
                if (overlap > 0)
                {
                    Vector2 correction = normal * overlap;
                    circle.X += correction.X;
                    circle.Y += correction.Y;
                }
                return;
            }

            if (object1 is CircleObject obj1 && object2 is CircleObject obj2)
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

                if (obj1.IsMain && obj2.IsMain)
                {
                    return;
                }

                if (obj1.IsMain)
                {
                    obj2.isDeleted = true;
                    SpawnPoints((int)obj2.Radius, obj2.X, obj2.Y);
                    SpawnBonus(obj2.X, obj2.Y);
                }
                else if (obj2.IsMain)
                {
                    obj1.isDeleted = true;
                    SpawnPoints((int)obj1.Radius, obj1.X, obj1.Y);
                    SpawnBonus(obj1.X, obj1.Y);
                }
                return;
            }

        }

        private void ApplyBonus(BonusObject bonus)
        {
            switch (bonus.Type)
            {
                case Types.pBalls:
                    AddBalls();
                    break;
                case Types.mBalls:
                    DeleteBalls();
                    break;
                case Types.pWidth:
                    ChangePlatformSize(100);
                    break;
                case Types.mWidth:
                    ChangePlatformSize(-100);
                    break;
                case Types.pTry:
                    stage.gameStat.Tries += 1;
                    break;
                case Types.mTry:
                    stage.gameStat.Tries += (stage.gameStat.Tries == 1) ? 0 : -1;
                    break;
                case Types.pSpeed:
                    platformSpeed += 5;
                    break;
                case Types.mSpeed:
                    platformSpeed -= 5;
                    break;
                case Types.pBSpeed:
                    ChangeBallsSpeed(3d);
                    break;
                case Types.mBSpeed:
                    ChangeBallsSpeed(-3d);
                    break;
            }
        }

        private void ChangePlatformSize(int delta)
        {
            foreach (var obj in stage.objects)
            {
                if (obj is RectangleObject rect && rect.isPlatform)
                {
                    rect.ChangeWidth(delta);
                    break;
                }
            }
        }

        private void ChangeBallsSpeed(double delta)
        {
            foreach (var obj in stage.objects)
            {
                if (obj is CircleObject circle && circle.IsMain)
                {
                    circle.Speed += delta;
                }
            }
        }

        private void AddBalls()
        {
            List<Tuple<double, double>> lst = new();
            foreach (var obj in stage.objects)
            {
                if (obj is CircleObject circle && circle.IsMain)
                {
                    //SpawnMainBall(circle.X, circle.Y);
                    lst.Add(new Tuple<double, double>(circle.X, circle.Y));
                }
            }
            foreach (var obj in lst)
            {
                SpawnMainBall(obj.Item1, obj.Item2);
            }
        }

        private void DeleteBalls()
        {
            int cnt = 0;
            foreach (var obj in stage.objects)
            {
                if (obj is CircleObject circle && circle.IsMain)
                {
                    cnt++;
                }
            }
            foreach (var obj in stage.objects)
            {
                if (cnt <= 1)
                    break;
                if (obj is CircleObject circle && circle.IsMain)
                {
                    circle.isDeleted = true;
                    cnt -= 2;
                }
            }
        }

        private void CheckLevelStatus()
        {
            var mainBalls = 0;
            var otherBalls = 0;
            foreach (var obj in stage.objects)
            {
                if (obj is CircleObject circle)
                {
                    if (circle.IsMain)
                        mainBalls++;
                    else otherBalls++;
                }
            }
            if (mainBalls == 0)
            {
                NewTry();
            }
            if (otherBalls == 0)
            {
                NewLevel(stage.gameStat.Level + 1);
            }
        }

        private void NewTry()
        {
            stage.gameStat.Tries--;
            if (stage.gameStat.Tries == -1)
                NewLevel(1);
            else
            {
                SpawnMainBall();
            }
            TogglePause();
        }

        private void NewLevel(int level)
        {
            stage.gameStat.Tries = 3;
            stage.gameStat.Level = level;
            if (level == 1)
                stage.gameStat.Points = 0;
            stage.ClearObjects();
            InitializeObjects();
        }

        private void SpawnBonus(double x, double y)
        {
            var type = (Types)random.Next(10);
            gameCanvas.Dispatcher.Invoke(() => stage.AddObject(new BonusObject(x, y, type)));
        }

        private void SpawnPoints(int points, double x, double y)
        {
            stage.gameStat.Points += points;
            gameCanvas.Dispatcher.Invoke(() =>
            {
                var point = new RectangleObject(x, y, 50, 50, Color.FromArgb(0, 0, 0, 0), points.ToString());
                point.ChangeBorder(Color.FromArgb(0, 0, 0, 0), 0);
                point.ChangeTextColor(Color.FromRgb(0, 0, 0));
                point.isPoints = true;
                point.Speed = 4;
                point.Angle = 90;
                stage.AddObject(point);
            });
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

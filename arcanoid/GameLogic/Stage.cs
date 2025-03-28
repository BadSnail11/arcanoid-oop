using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace arcanoid.GameLogic
{
    class Stage
    {
        public List<DisplayObject> objects = new List<DisplayObject>();
        public List<DisplayObject> menuObjects = new List<DisplayObject>();
        public List<DisplayObject> settingsObjects = new List<DisplayObject>();
        //public List<DisplayObject> gameObjects = new List<DisplayObject>();
        public RectangleObject border;
        private Canvas canvas;
        public Stage(Canvas gameCanvas)
        {
            canvas = gameCanvas;
            //border.borderSize = 10;
            //DrawBorder(gameCanvas);
        }
        public void AddObject(DisplayObject obj) => objects.Add(obj);
        public void RemoveObject(DisplayObject obj) => objects.Remove(obj);
        public void ClearObjects()
        {
            objects.Clear();
        }
        public void SaveObjectsToFile()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                InitialDirectory = @"C:\Users\Public\Documents\saves",
                DefaultExt = "json",
                Title = "Сохранить игру"
            };

            //if (saveFileDialog.ShowDialog() == true)
            //{
            //    string json = JsonSerializer.Serialize(stage.Objects, new JsonSerializerOptions { WriteIndented = true });
            //    File.WriteAllText(saveFileDialog.FileName, json);
            //}
            //var objectsData = objects.Select(obj => new
            //{
            //    Type = obj.GetType().Name,
            //    Json = obj
            //}).ToList();
            var objectsData = new List<object>();
            foreach (var obj in objects)
            {
                var type = obj.GetType().Name;
                if (type == "RectangleObject")
                {
                    var newObject = (RectangleObject)obj;
                    objectsData.Add(new
                    {
                        Type = obj.GetType().Name,
                        Json = newObject
                    });
                } else if (type == "CircleObject")
                {
                    var newObject = (CircleObject)obj;
                    objectsData.Add(new
                    {
                        Type = obj.GetType().Name,
                        Json = newObject
                    });
                } else if (type == "TrapezoidObject")
                {
                    var newObject = (TrapezoidObject)obj;
                    objectsData.Add(new
                    {
                        Type = obj.GetType().Name,
                        Json = newObject
                    });
                }
                else if (type == "TriangleObject")
                {
                    var newObject = (TriangleObject)obj;
                    objectsData.Add(new
                    {
                        Type = obj.GetType().Name,
                        Json = newObject
                    });
                }
            }
            if (saveFileDialog.ShowDialog() == true)
            {
                string json = JsonSerializer.Serialize(objectsData, new JsonSerializerOptions { WriteIndented = false });
                File.WriteAllText(saveFileDialog.FileName, json);
            }
        }
        public void LoadObjectsFromFile()
        {
            //if (!File.Exists(filename)) return;
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                InitialDirectory = @"C:\Users\Public\Documents\saves",
                Title = "Загрузить игру"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string json = File.ReadAllText(openFileDialog.FileName);
                var objectsData = JsonSerializer.Deserialize<List<dynamic>>(json);

                ClearObjects();

                foreach (var objData in objectsData)
                {
                    string jsonString = objData.GetProperty("Json").GetRawText();
                    string typeString = objData.GetProperty("Type").GetString();

                    DisplayObject obj;
                    switch (typeString)
                    {
                        case "RectangleObject":
                            obj = new RectangleObject().FromJson(jsonString);
                            break;
                        case "TriangleObject":
                            obj = new TriangleObject().FromJson(jsonString);
                            break;
                        case "TrapezoidObject":
                            obj = new TrapezoidObject().FromJson(jsonString);
                            break;
                        case "CircleObject":
                            obj = new CircleObject().FromJson(jsonString);
                            break;
                        default:
                            obj = null;
                            break;
                    };
                    //DisplayObject obj = DisplayObject.FromJson(jsonString, typeString);
                    if (obj != null) AddObject(obj);
                }
            }

        }
        public void Update(double canvasWidth, double canvasHeight, bool useAcceleration)
        {
            foreach (var obj in objects)
                obj.Move(canvasWidth, canvasHeight, useAcceleration);
        }
        public void DrawMenu(double canvasWidth, double canvasHeight)
        {
            var obj = menuObjects[0];
            obj.X = canvasWidth / 2 - 75;
            obj.Y = canvasHeight / 2 - 215;
            obj.Draw(canvas);
            for (int i = 1; i < menuObjects.Count; i++)
            {
                obj = menuObjects[i];
                obj.X = canvasWidth / 2 - 50;
                obj.Y = canvasHeight / 2 - 200 + (60 * (i - 1));
                obj.Draw(canvas);
            }
        }
        public void DrawSettings(double canvasWidth, double canvasHeight)
        {
            var obj = settingsObjects[0];
            obj.X = canvasWidth / 2 - 75;
            obj.Y = canvasHeight / 2 - 215;
            obj.Draw(canvas);
            for (int i = 1; i < settingsObjects.Count; i++)
            {
                obj = settingsObjects[i];
                obj.X = canvasWidth / 2 - 50;
                obj.Y = canvasHeight / 2 - 200 + (60 * (i - 1));
                obj.Draw(canvas);
            }
        }
        public void DrawBorder(double width, double height)
        {
            border = new RectangleObject(0, 0, width - 12, height - 13, Color.FromArgb(0, 255, 255, 255), "");
            //border.borderSize = 10;
            border.ChangeBorder(Color.FromRgb(0, 0, 255), 10);
            border.Draw(canvas);
        }
        public void Draw()
        {
            foreach(var obj in objects)
            {
                obj.Draw(canvas);
            }
            var a = 0;
        }
    }
}

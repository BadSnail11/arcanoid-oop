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

namespace arcanoid.GameLogic
{
    class Stage
    {
        public List<DisplayObject> objects = new List<DisplayObject>();
        public List<DisplayObject> menuObjects = new List<DisplayObject>();
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
        public void SaveObjectsToFile(string filename)
        {
            var objectsData = objects.Select(obj => new
            {
                Type = obj.GetType().Name,
                Json = obj.ToJson()
            }).ToList();

            string json = JsonSerializer.Serialize(objectsData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filename, json);
        }
        public void LoadObjectsFromFile(string filename)
        {
            if (!File.Exists(filename)) return;

            string json = File.ReadAllText(filename);
            var objectsData = JsonSerializer.Deserialize<List<dynamic>>(json);

            ClearObjects();

            foreach (var objData in objectsData)
            {
                string jsonString = objData.Json.GetRawText();
                string typeString = objData.Type.GetString();

                DisplayObject obj = DisplayObject.FromJson(jsonString, typeString);
                if (obj != null) AddObject(obj);
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
        public void DrawBorder(double width, double height)
        {
            border = new RectangleObject(0, 0, width - 12, height - 13, Color.FromArgb(0, 255, 255, 255), "");
            border.borderSize = 10;
            border.Draw(canvas);
        }
        public void Draw()
        {
            foreach(var obj in objects)
            {
                obj.Draw(canvas);
            }
        }
    }
}

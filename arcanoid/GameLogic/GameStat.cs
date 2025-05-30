using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Reflection.Emit;

namespace arcanoid.GameLogic
{
    class GameStat : DisplayObject
    {
        public int Level { get; set; }
        public int Points { get; set; }
        public int Tries { get; set; }
        [JsonIgnore]
        public RectangleObject Table { get; set; }

        public GameStat(int level, int points, int tries)
        {
            Level = level;
            Points = points;
            Tries = tries;
            Table = new RectangleObject(0, 50, 200, 50, Color.FromArgb(0, 0, 0, 0), $"level {level} | points {points} | tries {tries}");
            Table.ChangeBorder(Color.FromArgb(0, 0, 0, 0), 0);
            Table.ChangeTextColor(Color.FromRgb(0, 0, 0));
        }

        public override DisplayObject FromJson(string json)
        {
            return JsonSerializer.Deserialize<GameStat>(json);
        }

        public override void Draw(Canvas canvas)
        {
            Table.ChangeText($"level {Level} | points {Points} | tries {Tries}");
            Table.X = canvas.Width - 200 - 15;
            Table.Draw(canvas);
        }

        public override void ChangeBorder(Color color, double borderSize = 1)
        {
            Table.ChangeBorder(color, borderSize);
        }
    }
}

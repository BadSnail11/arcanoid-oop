using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public Game(Canvas gameCanvas)
        {
            stage = new Stage(gameCanvas);
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
            timer.Tick += (s, e) => { stage.Update(); stage.Draw(); };
        }
        public void Start() => timer.Start();
        public void Stop() => timer.Stop();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arcanoid.GameLogic
{
    class Game
    {
        private Stage stage = new Stage();
        private bool running = true;
        public void Start()
        {
            Task.Run(() =>
            {
                while (running)
                {
                    stage.update();
                    Thread.Sleep(16);
                }
            });
        }
        public void Stop() => running = false;
    }
}

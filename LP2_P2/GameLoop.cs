using System;
using System.Threading;

namespace LP2_P2
{
    class GameLoop
    {
        private Player player;
        private DoubleBuffer2D<char> db;
        private InputSystem inputSys;
        private bool running;

        public GameLoop()
        {
            player = new Player();
            db = new DoubleBuffer2D<char>(30, 30);
            inputSys = new InputSystem(player, db);
            Thread keyReader = new Thread(inputSys.ReadKeys);
            keyReader.Start();
            Console.CursorVisible = false;
        }

        public void Loop()
        {
            int msPerUpdate = 300000;
            long previous = Math.Abs(DateTime.Now.Ticks);
            long lag = 0L;

            running = true;
            while (running)
            {
                long current = Math.Abs(DateTime.Now.Ticks);
                long elapsed = current - previous;
                previous = current;
                lag += elapsed;

                inputSys.ProcessInput();
                while (lag >= msPerUpdate)
                {
                    inputSys.Update();
                    lag -= msPerUpdate;
                }
                Render();
            }
        }

        public void Render()
        {
            db[player.PosXOld, player.PosYOld] = ' ';
            db[player.PosX, player.PosY] = player.visuals;
            db.Swap();

            for (int y = 0; y < db.YDim; y++)
            {
                for (int x = 0; x < db.XDim; x++)
                {
                    Console.Write(db[x, y] + " ");
                }
                Console.WriteLine();
            }
            Console.SetCursorPosition(0, 0);
            db.Clear();
        }
    }
}

using System;
using System.Threading;

namespace GameServer
{
    class Program
    {
        // Variable to keep the program running
        private static bool running = false;

        // Variables for server initialisation
        public static int playerCount;
        public static int portNo;

        static void Main(string[] args)
        {
            Console.Title = "Game Server";
            running = true;

            // Ask user for the player count
            Console.WriteLine($"Server Starting...");
            Console.WriteLine($"Please enter the max number of players or leave blank for the default value: ");
            try
            {
                playerCount = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                playerCount = 2;
            }

            // Ask user for the port they wish to open the server on
            Console.WriteLine($"Please enter the port number you want to run the server on or leave blank for default: ");
            try
            {
                portNo = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                portNo = 26950;
            }

            Console.WriteLine($"~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            // Start the server with the given parameters
            Server.Start(playerCount, portNo);

        }

        // Use a thread to run the main loop so that it can be put to sleep 
        private static void MainThread()
        {
            Console.WriteLine($"Main thread started. Running at {Globals.TICKS_PER_SEC} ticks per second");
            DateTime _nextLoop = DateTime.Now;

            while (running)
            {
                // While the next loop is meant to start
                while (_nextLoop < DateTime.Now)
                {
                    // Update the game
                    GameRuntime.Update();

                    _nextLoop = _nextLoop.AddMilliseconds(Globals.MS_PER_TICK);
                }

                // If the next loop isn't meant to start yet then sleep until it is
                if (_nextLoop > DateTime.Now)
                {
                    Thread.Sleep(_nextLoop - DateTime.Now);
                }
            }
        }
    }
}

// Determines what happens during the runtime
namespace GameServer
{
    class GameRuntime
    {
        // Update the game
        public static void Update()
        {
            ThreadManager.UpdateMain();
        }
    }
}

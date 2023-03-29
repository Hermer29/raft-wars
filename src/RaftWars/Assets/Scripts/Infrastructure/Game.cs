using InputSystem;

namespace RaftWars.Infrastructure
{
    internal class Game
    {
        public static CollectiblesService CollectiblesService;
        public static bool Initialized { get; private set; }
        
        public Game()
        {
            CollectiblesService = new CollectiblesService();
            Initialized = true;
        }
    }
}
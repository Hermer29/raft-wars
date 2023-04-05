using DefaultNamespace;
using InputSystem;
using Interface;
using RaftWars.Infrastructure.AssetManagement;

namespace RaftWars.Infrastructure
{
    internal class Game
    {
        public static CollectiblesService CollectiblesService;
        public static PlayerService PlayerService;
        public static MaterialsService MaterialsService;
        public static AdvertisingService AdverisingService;
        public static FeatureFlags FeatureFlags;
        public static Hud Hud;
        public static InputService InputService;
        public static MapGenerator MapGenerator;

        public Game(Player player)
        {
            CollectiblesService = new CollectiblesService();
            PlayerService = new PlayerService(player);
            MaterialsService = new MaterialsService();
            AdverisingService = new AdvertisingService();
            FeatureFlags = AssetLoader.LoadFeatureFlags();
            Hud = GameFactory.CreateHud();
            InputService = new InputService(Hud.Joystick);
        }
    }
}
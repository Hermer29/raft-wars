using DefaultNamespace;
using InputSystem;
using Interface;
using RaftWars.Infrastructure.AssetManagement;
using Services;

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
        public static YandexIAPService IAPService;
        public static PlayerMoneyService MoneyService;
        public static PlayerUsingService UsingService;
        public static PropertyService PropertyService;
        public static GameManager GameManager;

        public Game(Player player)
        {
            CollectiblesService = new CollectiblesService();
            PlayerService = new PlayerService(player);
            AdverisingService = new AdvertisingService();
            FeatureFlags = AssetLoader.LoadFeatureFlags();
            Hud = GameFactory.CreateHud();
            InputService = new InputService(Hud.Joystick);
            IAPService = new YandexIAPService();
            MoneyService = new PlayerMoneyService(CrossLevelServices.PrefsService);
            PropertyService = new PropertyService(CrossLevelServices.PrefsService);
        }
    }
}
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
        public static StateMachine StateMachine;
        public static FightService FightService;

        public Game(Player player, StateMachine stateMachine, ICoroutineRunner coroutineRunner)
        {
            StateMachine = stateMachine;
            CollectiblesService = new CollectiblesService();
            PlayerService = new PlayerService(player);
            AdverisingService = new AdvertisingService();
            FeatureFlags = AssetLoader.LoadFeatureFlags();
            Hud = GameFactory.CreateHud();
            InputService = new InputService(Hud.Joystick);
            IAPService = new YandexIAPService();
            MoneyService = new PlayerMoneyService(CrossLevelServices.PrefsService, Hud);
            PropertyService = new PropertyService(CrossLevelServices.PrefsService);
            FightService = new FightService(
                new FightCameraService(GameFactory.CreatePlayerVirtualCamera()), PlayerService, coroutineRunner);
        }
    }
}
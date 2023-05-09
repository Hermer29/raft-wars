using DefaultNamespace;
using InputSystem;
using Interface;
using RaftWars.Infrastructure;
using Services;
using UnityEngine;

namespace Infrastructure
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
        public static Canvas StatsCanvas;
        public static AudioService AudioService;

        public Game(StateMachine stateMachine, ICoroutineRunner coroutineRunner)
        {
            StateMachine = stateMachine;
            CollectiblesService = new CollectiblesService();
            IAPService = new YandexIAPService();
            MoneyService = new PlayerMoneyService(CrossLevelServices.PrefsService);
            PropertyService = new PropertyService(CrossLevelServices.PrefsService);
            AdverisingService = new AdvertisingService(coroutineRunner);
            MaterialsService = new MaterialsService();
        }
    }
}
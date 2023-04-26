using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Infrastructure.Platforms;
using InputSystem;
using RaftWars.Infrastructure;
using RaftWars.Infrastructure.AssetManagement;
using RaftWars.Infrastructure.Services;
using RaftWars.Pickables;
using Services;
using SpecialPlatforms;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Infrastructure.States
{
    public class LoadGameplayState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly LoadingScreen _loadingScreen;
        private readonly ICoroutineRunner _coroutineRunner;

        public LoadGameplayState(StateMachine stateMachine, LoadingScreen loadingScreen, ICoroutineRunner coroutineRunner)
        {
            _loadingScreen = loadingScreen;
            _coroutineRunner = coroutineRunner;
            _stateMachine = stateMachine;
        }
        
        public void Exit()
        {
            
        }

        public void Enter()
        {
            Game.GameManager = GameFactory.CreateGameManager();
            Game.Hud = GameFactory.CreateHud();
            Game.InputService = new InputService(Game.Hud.Joystick);
            Game.AudioService = GameFactory.CreateAudioService();
            Game.AdverisingService.RewardedEnded += () => Game.AudioService.SetState(true);
            Game.AdverisingService.RewardedStarted += () => Game.AudioService.SetState(false);
            Game.MoneyService.AmountUpdated += Game.Hud.ShowCoins;
            Game.Hud.ShowCoins(Game.MoneyService.Amount);
            Game.StatsCanvas = GameFactory.CreateStatsCanvas();
            var platforms = AllServices.GetSingle<PlatformsFactory>().CreatePlatforms();
            AllServices.Register<IEnumerable<SpecialPlatform>>(platforms);
            Player player = CreatePlayer();
            Game.PlayerService = new PlayerService(player);
            Game.FightService = new FightService(
                new FightCameraService(GameFactory.CreatePlayerVirtualCamera()), 
                Game.PlayerService, 
                _coroutineRunner, 
                Game.AudioService,
                Resources.Load<FightConstants>("FightConstants"));
            var ownedPlatforms = platforms.Where(Game.PropertyService.IsOwned);
            var pickablesLoading = ownedPlatforms
                .Select(x => x.PickablePlatform);
            _coroutineRunner.StartCoroutine(WaitForSpecialPlatformsLoading(
                pickablesLoading,
                (result) =>
                {
                    Game.MapGenerator.Construct(Game.Hud.BossAppearing,
                        ownedPickable: result.Select(x => x.Result.GetComponent<Pickable>()));
                    Pause pause = GameFactory.CreatePauseMenu();
                    pause.Construct(Game.Hud.PauseButton, Game.FightService);
                    Game.GameManager.Construct(Game.MapGenerator, _stateMachine, Game.Hud.Arrow, Camera.main, pause);
                    Game.Hud.AdvertisingForStatsButton.Construct(Game.AdverisingService, Game.PlayerService, Game.GameManager);
                    CreateUsingService();
                    Game.Hud.SoundButton.Construct(Game.AudioService);
                    TryShowTutorial();
                    CreateSpecialPlatforms();

                    Game.PropertyService.PropertyOwned += newOwned =>
                    {
                        if (newOwned is SpecialPlatform)
                        {
                            _coroutineRunner.StartCoroutine(WaitForSpecialPlatformsLoading(
                                platforms.Where(Game.PropertyService.IsOwned).Select(x => x.PickablePlatform), result =>
                                {
                                    Game.MapGenerator.SetOwnedPickables(
                                        result.Select(x => x.Result.GetComponent<Pickable>()));
                                }));
                        }
                    };
                    if (Game.FeatureFlags.IMGUIEnabled)
                    { 
                        GameFactory.CreateIMGUI();
                    }
                    YandexIAPService yandexIapService = Game.IAPService;
                    PlayerMoneyService moneyService = Game.MoneyService;
                    PlayerUsingService usingService = Game.UsingService;
                    PropertyService propertyService = Game.PropertyService;
            
                    var uiAssets = new UiAssetLoader();
                    var uiFactory = new UiFactory(uiAssets, yandexIapService, moneyService, 
                        usingService, propertyService, _coroutineRunner);
                    Shop shop = uiFactory.CreateShop();
                    _loadingScreen.FadeOut();
                }));
        }

        private void CreateSpecialPlatforms()
        {
            var owningSequence = AllServices.GetSingle<OwningSequence<SpecialPlatform>>();
            foreach (SpecialPlatform platform in AllServices.GetSingle<IEnumerable<SpecialPlatform>>())
            {
                owningSequence.Register(platform);
            }
            AllServices.Register<PlatformsMenu>(AllServices.GetSingle<PlatformsFactory>()
                .CreatePlatformsMenu());
        }

        private IEnumerator WaitForSpecialPlatformsLoading(IEnumerable<AssetReference> owned, Action<IEnumerable<AsyncOperationHandle<GameObject>>> continuation)
        {
            var previousPercent = 2f;
            const float maximumPercent = 3;
            var loading = owned.Select(Addressables.LoadAssetAsync<GameObject>);
            while (loading.Any(x => x.IsDone == false))
            {
                float progress = previousPercent + loading.Average(x => x.PercentComplete);
                _loadingScreen.SetSliderProcess((previousPercent + progress) / maximumPercent);
                yield return null;
            }
            continuation.Invoke(loading);
        }

        private static void TryShowTutorial()
        {
            if (CrossLevelServices.PrefsService.GetInt("TutorialShown") == 0 || Game.FeatureFlags.TutorialEveryTime)
            {
                var tutorial = GameFactory.CreateTutorial();
                tutorial.Construct(Game.Hud.Joystick);
                CrossLevelServices.PrefsService.SetInt("TutorialShown", 1);
                Game.GameManager.GameStarted += tutorial.Show;
            }
        }

        private static void CreateUsingService()
        {
            Game.UsingService = new PlayerUsingService(
                Game.PlayerService, 
                CrossLevelServices.PrefsService);
        }

        private static Player CreatePlayer()
        {
            Player player = GameFactory.CreatePlayer();
            CinemachineVirtualCamera camera = GameFactory.CreatePlayerVirtualCamera();
            Transform transform = player.CameraGroup.transform;
            camera.m_Follow = transform;
            camera.m_LookAt = transform;
            var cameraOffset = camera.GetComponent<CinemachineCameraOffset>();
            player._camera = cameraOffset;
            player.Construct(AllServices.GetSingle<IEnumerable<SpecialPlatform>>());
            return player;
        }
    }
}
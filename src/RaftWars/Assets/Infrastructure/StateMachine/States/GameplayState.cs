using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DefaultNamespace.Skins;
using Infrastructure.Platforms;
using InputSystem;
using Interface;
using RaftWars.Infrastructure;
using RaftWars.Infrastructure.AssetManagement;
using RaftWars.Infrastructure.Services;
using RaftWars.Pickables;
using Services;
using SpecialPlatforms;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Object = System.Object;
using Random = UnityEngine.Random;

namespace Infrastructure.States
{
    public class GameplayState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly LoadingScreen _loadingScreen;
        private readonly ICoroutineRunner _coroutineRunner;

        public GameplayState(StateMachine stateMachine, LoadingScreen loadingScreen, ICoroutineRunner coroutineRunner)
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
            Game.MoneyService.AmountUpdated += Game.Hud.ShowCoins;
            Game.Hud.ShowCoins(Game.MoneyService.Amount);
            Game.StatsCanvas = GameFactory.CreateStatsCanvas();
            var platforms = AllServices.GetSingle<IEnumerable<SpecialPlatform>>();
            Player player = CreatePlayer();
            Game.PlayerService = new PlayerService(player);
            Game.FightService = new FightService(
                new FightCameraService(GameFactory.CreatePlayerVirtualCamera()), 
                Game.PlayerService, 
                _coroutineRunner, 
                Game.AudioService,
                Resources.Load<FightConstants>("FightConstants"));
            Game.Hud.AdvertisingForStatsButton.Construct(Game.AdverisingService, Game.PlayerService, Game.GameManager);
            var ownedPlatforms = platforms.Where(Game.PropertyService.IsOwned);
            var pickablesLoading = ownedPlatforms
                .Select(x => x.PickablePlatform);
            void Continuation(IEnumerable<AsyncOperationHandle<GameObject>> result)
            {
                var pickables = result.Select(x => x.Result.GetComponent<Pickable>());
                var metadata = pickables.Zip(ownedPlatforms, (loaded, meta) => (loaded, meta));
                AllServices.Register(pickables);
                Game.MapGenerator.Construct(Game.Hud.BossAppearing, 
                    ownedPickable: metadata);
                Pause pause = GameFactory.CreatePauseMenu();
                pause.Construct(Game.Hud.PauseButton, Game.FightService);
                Game.GameManager.Construct(Game.MapGenerator, _stateMachine, Game.Hud.Arrow, Camera.main, pause);
                CreateUsingService();
                Game.Hud.SoundButton.Construct(Game.AudioService);
                TryShowTutorial();
                CreateSpecialPlatforms();

                Game.PropertyService.PropertyOwned += newOwned =>
                {
                    if (newOwned is SpecialPlatform)
                    {
                        _coroutineRunner.StartCoroutine(WaitForSpecialPlatformsLoading(platforms.Where(Game.PropertyService.IsOwned).Select(x => x.PickablePlatform),
                            result =>
                            {
                                var pickables = result.Select(x => x.Result.GetComponent<Pickable>());
                                var metadata = pickables.Zip(ownedPlatforms, (loaded, meta) => (loaded, meta));
                                Game.MapGenerator.SetOwnedPickables(metadata);
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
                var uiFactory = new UiFactory(uiAssets, yandexIapService, moneyService, usingService, propertyService, _coroutineRunner);
                Shop shop = uiFactory.CreateShop();
                _loadingScreen.FadeOut();
                Game.Hud.ShowBonusWindow(() => Game.GameManager.StartGame());
                ShowRewardsWindow();
                BindAllButtonsToAdvertisingShow();
            }

            _coroutineRunner.StartCoroutine(WaitForSpecialPlatformsLoading(
                pickablesLoading,
                Continuation));
        }

        private void BindAllButtonsToAdvertisingShow()
        {
            foreach (var button in UnityEngine.Object.FindObjectsOfType<Button>(true)
                         .Where(x => x.GetComponent<IgnoreInterstitialButtonMarker>() == null))
            {
                button.onClick.AddListener(() => Game.AdverisingService.ShowInterstitial());
            }
        }

        private static void ShowRewardsWindow()
        {
            const string firstPlayKey = "FirstStart_RewardsWindow";
            var isFirstStart = CrossLevelServices.PrefsService.GetInt(firstPlayKey, 0) == 0;
            if (isFirstStart == false)
            {
                if (Random.Range(0, 100) > 10)
                    MakeRewardWindow();
            }
            CrossLevelServices.PrefsService.SetInt(firstPlayKey, 1);
        }

        private static void MakeRewardWindow()
        {
            Game.Hud.HideBonusWindow();
            var hats = AssetLoader.LoadHatSkins().Cast<IShopProduct>();
            var colors = AssetLoader.LoadPlatformSkins().Cast<IShopProduct>();
            var allSkins = hats.Concat(colors);
            var rewardWindowProcessing = new RewardWindowProcessing(allSkins, Game.PropertyService, Game.AdverisingService, Game.UsingService);
            rewardWindowProcessing.Hidden += () => Game.Hud.ShowBonusWindow(() => Game.GameManager.StartGame());
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
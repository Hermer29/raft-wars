using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinemachine;
using Infrastructure.Platforms;
using Infrastructure.States;
using InputSystem;
using LanguageChanger;
using RaftWars.Infrastructure.AssetManagement;
using RaftWars.Infrastructure.Services;
using RaftWars.Pickables;
using SpecialPlatforms;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace RaftWars.Infrastructure
{
    public class CreateServicesState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly LoadingScreen _loadingScreen;
        private readonly ICoroutineRunner _coroutineRunner;

        public CreateServicesState(StateMachine stateMachine, LoadingScreen loadingScreen, ICoroutineRunner coroutineRunner)
        {
            _loadingScreen = loadingScreen;
            _coroutineRunner = coroutineRunner;
            _stateMachine = stateMachine;
        }
        
        public void Exit()
        {
            _loadingScreen.FadeOut();
        }

        public void Enter()
        {
            AllServices.Register<DescriptionProvider>(Object.FindObjectOfType<DescriptionProvider>());
            AllServices.Register<SaveService>(new SaveService(
                _coroutineRunner, CrossLevelServices.PrefsService));
            AllServices.Register<PlatformsLoader>(new PlatformsLoader());
            AllServices.Register<PlatformsFactory>(new PlatformsFactory(
                AllServices.GetSingle<SaveService>(),
                AllServices.GetSingle<PlatformsLoader>()));
            var platforms = AllServices.GetSingle<PlatformsFactory>().CreatePlatforms();
            AllServices.Register<IEnumerable<SpecialPlatform>>(platforms);
            
            var materialService = new MaterialsService();
            Player player = CreatePlayer();
            Game.MaterialsService = materialService;
            var game = new Game(player, _stateMachine, _coroutineRunner);
            Game.GameManager = GameFactory.CreateGameManager();

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
                    CreateSpecialPlatformsService();

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
                    _stateMachine.Enter<CreateIMGUIState>();
                    _stateMachine.Enter<CreateShopState>();
                }));
        }

        private void CreateSpecialPlatformsService()
        {
            var owningSequence = new OwningSequence<SpecialPlatform>(Game.PropertyService, Game.FeatureFlags);
            AllServices.Register<OwningSequence<SpecialPlatform>>(owningSequence);
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
            //new GameObject().AddComponent<HUDsScaler>().Construct(cameraOffset);
            player.Construct();
            return player;
        }
    }
}
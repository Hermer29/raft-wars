using Cinemachine;
using Infrastructure.States;
using InputSystem;
using UnityEngine;
using Visual;

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
            var materialService = new MaterialsService();
            Player player = CreatePlayer();
            Game.MaterialsService = materialService;
            var game = new Game(player, _stateMachine, _coroutineRunner);
            Game.GameManager = GameFactory.CreateGameManager();
            Game.MapGenerator.Construct();
            var pause = GameFactory.CreatePauseMenu();
            pause.Construct(Game.Hud.PauseButton);
            Game.GameManager.Construct(Game.MapGenerator, _stateMachine, Game.Hud.Arrow, Camera.main, pause);
            Game.UsingService = new PlayerUsingService(Game.PlayerService, CrossLevelServices.PrefsService);
            
            if(CrossLevelServices.PrefsService.GetInt("TutorialShown", 0) == 0 || Game.FeatureFlags.TutorialEveryTime)
            {
                var tutorial = GameFactory.CreateTutorial();
                tutorial.Construct(Game.Hud.Joystick);
                CrossLevelServices.PrefsService.SetInt("TutorialShown", 1);
                Game.GameManager.GameStarted += tutorial.Show;
            }

            _stateMachine.Enter<CreateIMGUIState>();
            _stateMachine.Enter<CreateShopState>();
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
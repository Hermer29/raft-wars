using Cinemachine;
using Infrastructure.States;
using InputSystem;
using UnityEngine;

namespace RaftWars.Infrastructure
{
    public class CreateServicesState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly LoadingScreen _loadingScreen;

        public CreateServicesState(StateMachine stateMachine, LoadingScreen loadingScreen)
        {
            _loadingScreen = loadingScreen;
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
            var game = new Game(player);
            Game.GameManager = GameFactory.CreateGameManager();
            Game.MapGenerator.Construct();
            Game.GameManager.Construct(Game.MapGenerator, _stateMachine);
            Game.UsingService = new PlayerUsingService(Game.PlayerService, CrossLevelServices.PrefsService);
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
            player._camera = camera.GetComponent<CinemachineCameraOffset>();
            player.Construct();
            return player;
        }
    }
}
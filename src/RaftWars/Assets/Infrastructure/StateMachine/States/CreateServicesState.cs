using Cinemachine;
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
            Player player = CreatePlayer();
            var game = new Game(player);
            GameManager manager = GameFactory.CreateGameManager();
            Game.MapGenerator.Construct();
            manager.Construct(Game.MapGenerator, _stateMachine);
            _stateMachine.Enter<CreateIMGUIState>();
        }

        private static Player CreatePlayer()
        {
            Player player = GameFactory.CreatePlayer();
            CinemachineVirtualCamera camera = GameFactory.CreatePlayerVirtualCamera();
            Transform transform = player.CameraGroup.transform;
            camera.m_Follow = transform;
            camera.m_LookAt = transform;
            player._camera = camera.GetComponent<CinemachineCameraOffset>();
            return player;
        }
    }
}
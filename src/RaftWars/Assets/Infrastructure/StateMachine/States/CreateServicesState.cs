using System.Linq;
using Cinemachine;
using Infrastructure.States;
using InputSystem;
using RaftWars.Infrastructure.AssetManagement;
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
            Player player = CreatePlayer(materialService);
            Game.MaterialsService = materialService;
            var game = new Game(player);
            Game.GameManager = GameFactory.CreateGameManager();
            Game.MapGenerator.Construct();
            Game.GameManager.Construct(Game.MapGenerator, _stateMachine);
            Game.UsingService = new PlayerUsingService(Game.PlayerService);
            _stateMachine.Enter<CreateIMGUIState>();
            _stateMachine.Enter<CreateShopState>();
        }

        private static Player CreatePlayer(MaterialsService materialsService)
        {
            Player player = GameFactory.CreatePlayer();
            CinemachineVirtualCamera camera = GameFactory.CreatePlayerVirtualCamera();
            Transform transform = player.CameraGroup.transform;
            camera.m_Follow = transform;
            camera.m_LookAt = transform;
            player._camera = camera.GetComponent<CinemachineCameraOffset>();
            player.Construct(materialsService.GetPlayerMaterial());
            
            player.ApplyHat(AssetLoader.LoadHatSkins().ElementAt(3));
            player.ApplyPlatformSkin(AssetLoader.LoadPlatformSkins().ElementAt(3));
            player.RepaintWith(AssetLoader.LoadPlayerColors().ElementAt(3));
            
            return player;
        }
    }
}
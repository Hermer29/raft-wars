using System.Collections;
using RaftWars.Infrastructure;
using RaftWars.Infrastructure.Services;
using TurretMinigame;
using TurretMinigame.Player;
using TurretMinigame.Service;
using UnityEngine;

namespace Infrastructure.States
{
    public class TurretMinigameState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly LoadingScreen _loadingScreen;
        private ICoroutineRunner _coroutineRunner;
        private IPrefsService _prefsService;
        private MinigameTurretInputService _inputService;
        private MinigamePlatform _platform;
        private MinigameTurret _turret;

        private const string PlayerOwningTurret = "PlayerOwning";

        public TurretMinigameState(StateMachine stateMachine, LoadingScreen loadingScreen)
        {
            _stateMachine = stateMachine;
            _loadingScreen = loadingScreen;
        }
        
        public void Enter()
        {
            ParseServices();
            var turretMinigameFactory = new TurretMinigameFactory(new TurretMinigameAssetLoader());
            int playersTurretIndex = _prefsService.GetInt(PlayerOwningTurret, 1);
            _platform = turretMinigameFactory.CreateMinigamePlatform();
            _turret = turretMinigameFactory.CreateTurret(playersTurretIndex);
            _turret.Construct(turretMinigameFactory);
            
            _inputService.HorizontalDeltaPositionUpdated += _turret.Rotate;
            _platform.PlaceTurret(_turret);
            _platform.PlayingCamera.Priority = 10;
            _loadingScreen.FadeOut();
            _turret.StartShooting();

            _platform.Generator.PlayerWon += EndGame;
            _platform.Generator.PlayerLost += EndGame;
            _platform.Generator.StartGeneration();
        }

        private void EndGame()
        {
            _stateMachine.Enter<LoadLevelState, int>(CrossLevelServices.LevelService.Level);
        }

        private void ParseServices()
        {
            _coroutineRunner = AllServices.GetSingle<ICoroutineRunner>();
            _prefsService = AllServices.GetSingle<IPrefsService>();
            _inputService = new MinigameTurretInputService(_coroutineRunner);
        }

        public void Exit()
        {
            Object.Destroy(_turret);
            Object.Destroy(_platform);
        }
    }
}
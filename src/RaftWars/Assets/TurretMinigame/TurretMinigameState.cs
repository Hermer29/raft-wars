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

        private const string PlayerOwningTurret = "PlayerOwning";
        private const float AttackCooldown = .3f;

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
            MinigamePlatform platform = turretMinigameFactory.CreateMinigamePlatform();
            MinigameTurret turret = turretMinigameFactory.CreateTurret(playersTurretIndex);
            _inputService.HorizontalDeltaPositionUpdated += turret.Rotate;
            
            platform.PlaceTurret(turret);
            platform.PlayingCamera.Priority = 10;
            _loadingScreen.FadeOut();
            _coroutineRunner.StartCoroutine(ShootOverTime());
        }

        private void ParseServices()
        {
            _coroutineRunner = AllServices.GetSingle<ICoroutineRunner>();
            _prefsService = AllServices.GetSingle<IPrefsService>();
            _inputService = new MinigameTurretInputService(_coroutineRunner);
        }

        private IEnumerator ShootOverTime()
        {
            while (true)
            {
                yield return new WaitForSeconds(AttackCooldown);
                
            }
        }

        public void Exit()
        {
            
        }
    }
}
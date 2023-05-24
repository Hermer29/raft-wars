using System;
using System.Collections;
using RaftWars.Infrastructure;
using RaftWars.Infrastructure.Services;
using TurretMinigame;
using TurretMinigame.Player;
using TurretMinigame.Service;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Infrastructure.States
{
    public class TurretMinigameState : IState
    {
        private readonly StateMachine _stateMachine;
        private ICoroutineRunner _coroutineRunner;
        private IPrefsService _prefsService;
        private MinigameTurretInputService _inputService;
        private MinigamePlatform _platform;
        private MinigameTurret _turret;
        private TurretMinigameHud _hud;
        private float _startTime;
        private TurretMinigameFactory _turretMinigameFactory;
        private AudioService _audioService;

        private const string PlayerOwningTurret = "PlayerOwning";
        private const float CoinsPerKill = 10f;

        public TurretMinigameState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        private void ParseServices()
        {
            _coroutineRunner = AllServices.GetSingle<ICoroutineRunner>();
            _prefsService = AllServices.GetSingle<IPrefsService>();
            _inputService = new MinigameTurretInputService(_coroutineRunner);
            _audioService = Game.AudioService;
        }

        public void Enter()
        {
            ParseServices();
            _turretMinigameFactory = new TurretMinigameFactory(new TurretMinigameAssetLoader());
            _hud = _turretMinigameFactory.CreateTurretMinigameHud();
            _platform = _turretMinigameFactory.CreateMinigamePlatform();
            CreateTurretWithAdvertisingUpgradeOption();
            _platform.PlayingCamera.Priority = 0;
            _platform.LookingAtTurretCamera.Priority = 1;
            _hud.ClickedOnScreen += StartGame;
        }

        private void CreateTurretWithAdvertisingUpgradeOption()
        {
            int currentTier = _prefsService.GetInt(PlayerOwningTurret, 1);
            _turret = _turretMinigameFactory.CreateTurret(currentTier);
            _turret.Construct(_turretMinigameFactory, _audioService);
            _platform.PlaceTurret(_turret);
            if (_turretMinigameFactory.TryCreateTurretOfTier(currentTier + 1, out _))
            {
                _hud.UpgradeTurretForAdvertising.onClick.AddListener(() =>
                {
                    Game.AdverisingService.ShowRewarded(() =>
                    {
                        _prefsService.SetInt(PlayerOwningTurret, currentTier + 1);
                        _hud.HideAdvertisingOffer();
                        CreateTurret();
                    });
                });
                _hud.ShowAdvertisingOffer(_turret.Illustration);
            }
        }

        private void CreateTurret()
        {
            if (_turret != null)
            {
                Object.Destroy(_turret.gameObject);
            }
            int currentTier = _prefsService.GetInt(PlayerOwningTurret, 1);
            _turret = _turretMinigameFactory.CreateTurret(currentTier);
            _turret.Construct(_turretMinigameFactory, _audioService);
            _platform.PlaceTurret(_turret);
        }

        private void StartGame()
        {
            _startTime = Time.time;
            _hud.HideAdvertisingOffer();
            _turret.StartShooting();
            _inputService.HorizontalDeltaPositionUpdated += _turret.Rotate;
            _platform.LookingAtTurretCamera.Priority = 0;
            _platform.PlayingCamera.Priority = 1;
            _platform.Generator.Construct(_turret, _hud.PlayerEnemiesView, _audioService);
            _platform.Generator.PlayerWon += OnWon;
            _platform.Generator.PlayerLost += OnLost;
            _platform.Generator.StartGeneration();
            _hud.PlayerEnemiesView.gameObject.SetActive(true);
        }

        private void OnWon()
        {
            EndGame();
        }

        private void OnLost()
        {
            _turret.BreakTower();
            EndGame();
        }

        private void EndGame()
        {
            _turret.StopShooting();
            _hud.PlayerEnemiesView.Hide();
            var coins = (int)(_platform.Generator.KillCount * CoinsPerKill);
            var coinsForAdvertising = (int)(coins * 3);
            _hud.ShowMenu(_platform.Generator.KillCount, 
                (int)(Time.time - _startTime),
                coins,
                coinsForAdvertising
            );
            _platform.LookingAtTurretCamera.Priority = 1;
            _platform.PlayingCamera.Priority = 0;
            _hud.TakeNormalAmount.onClick.AddListener(() =>
            {
                Game.MoneyService.AddCoins(coins);
                Continue();
            });
            _hud.TakeAdvertisingAmount.onClick.AddListener(() =>
            {
                Game.AdverisingService.ShowRewarded(() =>
                {
                    Game.MoneyService.AddCoins(coinsForAdvertising);
                    Continue();
                });
            });
        }

        private void Continue()
        {
            _stateMachine.Enter<LoadLevelState, int>(CrossLevelServices.LevelService.Level);
        }

        public void Exit()
        {
            _inputService.HorizontalDeltaPositionUpdated -= _turret.Rotate;
            _platform.Generator.PlayerWon -= EndGame;
            _platform.Generator.PlayerLost -= EndGame;
            Object.Destroy(_turret);
            Object.Destroy(_platform);
        }
    }
}
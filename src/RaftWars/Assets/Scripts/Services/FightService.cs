using System;
using System.Collections;
using InputSystem;
using RaftWars.Infrastructure;

namespace Services
{
    public class FightService
    {
        private readonly FightCameraService _fightCamera;
        private readonly PlayerService _player;
        private readonly ICoroutineRunner _runner;
        private Enemy _currentFightEnemy;

        private const float DamageModifier = .5f;
        
        public FightService(FightCameraService fightCameraService, PlayerService player, ICoroutineRunner runner)
        {
            _fightCamera = fightCameraService;
            _player = player;
            _runner = runner;
        }

        public bool FightStarted { get; private set; }

        public void Start(Enemy enemy)
        {
            if (FightStarted)
                throw new InvalidOperationException();
            FightStarted = true;
            
            _currentFightEnemy = enemy;
            _fightCamera.FightStarted(_player.PlayerInstance, enemy);
        }

        private IEnumerator FightProcess()
        {
            yield return null;
        }

        public void End()
        {
            if (FightStarted == false)
                throw new InvalidOperationException();
            FightStarted = false;
            
            _fightCamera.FightEnded();
        }
    }
}
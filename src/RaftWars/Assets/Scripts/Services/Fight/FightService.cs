using System;
using System.Collections;
using Common;
using InputSystem;
using RaftWars.Infrastructure;
using UnityEngine;

namespace Services
{
    public class FightService
    {
        private readonly FightCameraService _fightCamera;
        private readonly PlayerService _player;
        private readonly ICoroutineRunner _runner;
        private Enemy _currentFightEnemy;
        private AudioService _audioService;
        private FightConstants _fightConstants;
        private bool _paused;

        //private IEnumerable<FightingUnit> _enemySideUnits; TODO
        //private IEnumerable<FightingUnit> _playerSideUnits; TODO

        public FightService(FightCameraService fightCameraService, PlayerService player, ICoroutineRunner runner,
            AudioService audioService, FightConstants fightConstants)
        {
            _audioService = audioService;
            _fightCamera = fightCameraService;
            _player = player;
            _runner = runner;
            _fightConstants = fightConstants;
        }

        public bool FightStarted { get; private set; }

        public void FightBeginningCollisionDetected(Enemy enemy)
        {
            //CreateFightingUnits(enemy); COMING SOON TODO
            if (FightStarted)
                return;
            FightStarted = true;
            
            _currentFightEnemy = enemy;
            _fightCamera.FightStarted(_player.PlayerInstance, enemy);
            _runner.StartCoroutine(PlayerFightProcess());
            _runner.StartCoroutine(EnemyFightProcess());
            enemy.StartFight();
            _audioService.PlayFightAudio();
        }

        public void TryPause()
        {
            _paused = true;
        }

        public void TryUnpause()
        {
            _paused = false;
        }

        private void CreateFightingUnits(Enemy enemy)
        {
            // _enemySideUnits = enemy.warriors.Select(x => new FightingUnit(1f, _runner));
            // _playerSideUnits = _player.PlayerInstance.warriors.Select(x => new FightingUnit(1f, _runner));
            // foreach (var warrior in enemy.warriors)
            // {
            // }
        }

        private IEnumerator PlayerFightProcess()
        {  
            do
            {
                var attackFrequency = CalculateAttackFrequency(CalculatePlayerSuperiority());
                Debug.Log("Player af:" + attackFrequency);
                yield return new WaitForSeconds(attackFrequency - _fightConstants.PlayerAttackSpeedAmplification);
                if (FightStarted == false)
                    break;
                while(_paused)
                    yield return null;
                _currentFightEnemy.DealDamage();
            } while (IsParticipantsAlive());
            End();
        }

        private IEnumerator EnemyFightProcess()
        {
            do
            {
                var attackFrequency = CalculateAttackFrequency(-CalculatePlayerSuperiority());
                Debug.Log("Enemy af:" + attackFrequency);
                yield return new WaitForSeconds(attackFrequency);
                if (FightStarted == false)
                    break;
                while(_paused)
                    yield return null;
                _player.DealDamage();
            } while (IsParticipantsAlive());
        }

        public float CalculatePlayerSuperiority()
        {
            return _player.PlayerInstance.damage - _currentFightEnemy.damage;
        }

        private float CalculateAttackFrequency(float superiority)
        {
            float fromDifference = superiority * _fightConstants.DifferenceWeight;
            float fromDamage = CalculateFromOverallDamageFactor();
            if(fromDifference > 0)
            {
                return Mathf.Abs(_fightConstants.FightSpeedModifierDecreasing / (fromDifference + fromDamage));
            }
            return Mathf.Abs((_fightConstants.FightSpeedModifierDecreasing + fromDifference) / fromDamage);
        }

        private float CalculateFromOverallDamageFactor()
        {
            return (_currentFightEnemy.damage + _player.PlayerInstance.damage) * _fightConstants.DamageWeight;
        }
        
        private bool IsParticipantsAlive()
        {
            return _player.PlayerInstance.hp > 0 && Mathf.Floor(_currentFightEnemy.hp) > 0;
        }

        private FighterRaft Winner()
        {
            if (IsParticipantsAlive())
                throw new InvalidOperationException();

            if (_player.PlayerInstance.hp > 0)
                return _player.PlayerInstance;
            return _currentFightEnemy;
        }

        private FighterRaft Looser()
        {
            if (_player.PlayerInstance == Winner())
                return _currentFightEnemy;
            return _player.PlayerInstance;
        }

        private void End()
        {
            _audioService.StopFightAudio();
            Winner().StopFight();
            Looser().Die();
            _currentFightEnemy = null;
            _fightCamera.FightEnded();
            FightStarted = false;
        }
    }
}
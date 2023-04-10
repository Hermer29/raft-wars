﻿using System;
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

        public FightService(FightCameraService fightCameraService, PlayerService player, ICoroutineRunner runner)
        {
            _fightCamera = fightCameraService;
            _player = player;
            _runner = runner;
        }

        public bool FightStarted { get; private set; }

        public void FightBeginningCollisionDetected(Enemy enemy)
        {
            if (FightStarted)
                return;
            FightStarted = true;
            
            _currentFightEnemy = enemy;
            _fightCamera.FightStarted(_player.PlayerInstance, enemy);
            _runner.StartCoroutine(PlayerFightProcess());
            _runner.StartCoroutine(EnemyFightProcess());
        }
        
        private IEnumerator PlayerFightProcess()
        {  
            do
            {
                yield return new WaitForSeconds(CalculatePlayerAttackFrequency());
                _currentFightEnemy.DealDamage();
            } while (IsParticipantsAlive());
        }

        private IEnumerator EnemyFightProcess()
        {
            do
            {
                yield return new WaitForSeconds(CalculateEnemyAttackFrequency());
                _player.DealDamage();
            } while (IsParticipantsAlive());
            End();
        }

        private float CalculatePlayerSuperiority()
        {
            return _player.PlayerInstance.damage - _currentFightEnemy.damage;
        }

        private float CalculateEnemyAttackFrequency()
        {
            float fromDifference = CalculatePlayerSuperiority() * FightConstants.DifferenceWeight;
            float fromDamage = _currentFightEnemy.damage * FightConstants.DamageWeight;
            return MathF.Abs((fromDamage + fromDifference) * FightConstants.Damage2AttackTime);
        }

        private float CalculatePlayerAttackFrequency()
        {
            float fromDifference = -CalculatePlayerSuperiority() * FightConstants.DifferenceWeight;
            float fromDamage = _player.PlayerInstance.damage * FightConstants.DamageWeight;
            return MathF.Abs((fromDamage + fromDifference) * FightConstants.Damage2AttackTime);
        }

        private bool IsParticipantsAlive()
        {
            return _player.PlayerInstance.hp > 0 && _currentFightEnemy.hp > 0;
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
            Winner().StopFight();
            Looser().Die();
            _currentFightEnemy = null;
            _fightCamera.FightEnded();
            FightStarted = false;
        }
    }
}
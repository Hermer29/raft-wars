using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using RaftWars.Infrastructure;
using UnityEngine;

namespace Services
{
    public class FightingUnit
    {
        private float _health;
        private bool _defeated;
        private FightingUnit _currentTarget;
        private IEnumerable<FightingUnit> _targets;
        
        private readonly float _attackCooldown;
        private readonly int _damage = 1;
        private readonly ICoroutineRunner _runner;
        
        public FightingUnit(float attackCooldown, ICoroutineRunner runner)
        {
            _attackCooldown = attackCooldown;
        }

        public bool Defeated => _defeated;

        public event Action Died;

        public void StartFighting(IEnumerable<FightingUnit> targets)
        {
            _targets = targets;
            _runner.StartCoroutine(FightingProcessCoroutine());
        }

        private IEnumerator FightingProcessCoroutine()
        {
            void AssignNewRandomTarget()
            {
                _currentTarget = _targets.Where(unit => unit.Defeated == false)
                    .Random();
            }

            while (ExistsAliveTarget())
            {
                AssignNewRandomTarget();
                while (_currentTarget.Defeated == false)
                {
                    if (_defeated)
                        yield break;
                    _currentTarget.DealDamage(_damage);
                    yield return new WaitForSeconds(_attackCooldown);
                }
            }
        }

        private bool ExistsAliveTarget()
        {
            return _targets.Any(x => x.Defeated == false);
        }

        private void DealDamage(int damage)
        {
            _health -= damage;
            if (!(_health <= 0)) return;
            _defeated = true;
            Died?.Invoke();
        }
    }
}
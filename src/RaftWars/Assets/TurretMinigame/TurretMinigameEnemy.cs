using System;
using System.Collections;
using System.Collections.Generic;
using TurretMinigame.Enemies;
using TurretMinigame.Player;
using UnityEngine;

namespace TurretMinigame
{
    public class TurretMinigameEnemy : MonoBehaviour
    {
        private const int DamagePerSecond = 1;
        private const int HitsToKill = 3;
        
        [SerializeField] private float _speed;
        [SerializeField] private Animator _animator;
        [SerializeField] private ParticleSystem _shotParticles;
        [SerializeField] private AudioSource _shootingSound;
        
        private Queue<(Vector3, string)> _wayPoints = new Queue<(Vector3, string)>();
        private EnemiesGenerator _generator;
        private int _counter = HitsToKill;
        private MinigameTurret _turret;
        private bool _attackInProgess;
        private Coroutine _shootingOverTime;
        private AudioService _audioService;

        public void Construct(EnemiesGenerator generator, MinigameTurret turret, AudioService audioService)
        {
            _audioService = audioService;
            _turret = turret;
            _generator = generator;
            _animator.Play("Swim");
        }
        
        public void SetupWaypoints(Vector3 XYWaypoint, string animationName)
        {
            _wayPoints.Enqueue((XYWaypoint, animationName));
        }
        
        private IEnumerator ShotOverTime()
        {
            while (true)
            {
                if(_generator.GameEnded)
                    yield break;
                _shotParticles.Stop();
                _shotParticles.Play();
                _shootingSound.Stop();
                _shootingSound.Play();
                yield return new WaitForSeconds(1.5f);
            }
        }

        private void Update()
        {
            if (_generator.GameEnded)
            {
                _animator.Play("Idle");
                return;
            }
            if (_attackInProgess)
            {
                _generator.DealDamage(Time.deltaTime * DamagePerSecond);
                return;
            }
            bool IsDestinationNear()
            {
                Vector3 vectorTowardsWaypoint = _wayPoints.Peek().Item1 - transform.position;
                const float CriticalDistance = 20;
                return vectorTowardsWaypoint.sqrMagnitude < CriticalDistance;
            }
            
            if (_wayPoints.Count == 0)
            {
                StartAttackingPlayer();
                return;
            }

            Vector3 waypoint = _wayPoints.Peek().Item1;
            Vector3 position = transform.position;
            position = Vector3.MoveTowards(position,
                new Vector3(waypoint.x, waypoint.y, position.z), 
                Time.deltaTime * _speed);
            transform.position = position;

            if (IsDestinationNear())
            {
                _wayPoints.Dequeue();
                if (_wayPoints.Count != 0)
                    _animator.Play(_wayPoints.Peek().Item2);
            }
        }

        private void StartAttackingPlayer()
        {
            _shootingOverTime = StartCoroutine(ShotOverTime());
            _animator.Play("Shoot");
            GetComponentInChildren<Rigidbody>().isKinematic = true;
            _attackInProgess = true;
        }

        private void OnCollisionStay(Collision collision)
        {
            DetectEnemyWhichFinishedThePath(collision);
        }

        private void DetectEnemyWhichFinishedThePath(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("MinigameEnemies"))
            {
                var other = collision.gameObject.GetComponent<TurretMinigameEnemy>();
                if(other._attackInProgess)
                    StartAttackingPlayer();
            }
        }

        private void OnDestroy()
        {
            TryStopShooting();
        }

        private void TryStopShooting()
        {
            if (_shootingOverTime == null)
                return;

            _shotParticles.Stop();
            StopCoroutine(_shootingOverTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            DetectEnemyWhichFinishedThePath(collision);
        }
        
        public void TakeHit(GameObject bullet)
        {
            _audioService.PlayHitMarker();
            _turret.ReturnBullet(bullet);
            _counter--;
            if (_counter == 0)
            {
                _generator.OneDied();
                Destroy(gameObject);
            }
        }
    }
}
using System.Collections.Generic;
using TurretMinigame.Enemies;
using TurretMinigame.Player;
using UnityEngine;

namespace TurretMinigame
{
    public class TurretMinigameEnemy : MonoBehaviour
    {
        private const int DamagePerSecond = 1;
        [SerializeField] private float _speed;
        [SerializeField] private Animator _animator;
        
        private Queue<Vector3> _wayPoints = new Queue<Vector3>();
        private EnemiesGenerator _generator;
        private int _counter = 4;
        private MinigameTurret _turret;
        private bool _attackInProgess;

        public void Construct(EnemiesGenerator generator, MinigameTurret turret)
        {
            _turret = turret;
            _generator = generator;
            _animator.Play("Running");
        }
        
        public void SetupWaypoints(Vector3 XYWaypoint)
        {
            _wayPoints.Enqueue(XYWaypoint);
        }

        private void Update()
        {
            if (_generator.GameEnded)
            {
                return;
            }
            if (_attackInProgess)
            {
                _generator.DealDamage(Time.deltaTime * DamagePerSecond);
                return;
            }
            bool IsDestinationNear()
            {
                Vector3 vectorTowardsWaypoint = _wayPoints.Peek() - transform.position;
                const float CriticalDistance = 20;
                return vectorTowardsWaypoint.sqrMagnitude < CriticalDistance;
            }
            
            if (_wayPoints.Count == 0)
            {
                StartAttackingPlayer();
                return;
            }

            Vector3 waypoint = _wayPoints.Peek();
            Vector3 position = transform.position;
            position = Vector3.MoveTowards(position,
                new Vector3(waypoint.x, waypoint.y, position.z), 
                Time.deltaTime * _speed);
            transform.position = position;

            if (IsDestinationNear())
            {
                _wayPoints.Dequeue();
            }
        }

        private void StartAttackingPlayer()
        {
            _animator.Play("Shoot");
            GetComponentInChildren<Rigidbody>().isKinematic = true;
            _attackInProgess = true;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
            {
                _turret.ReturnBullet(collision.gameObject);
                _counter--;
                if (_counter == 0)
                {
                    _generator.OneDied();
                    Destroy(gameObject);
                }
            }
        }
    }
}
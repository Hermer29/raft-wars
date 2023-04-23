using System;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core;
using UnityEngine;

namespace TurretMinigame
{
    public class TurretMinigameEnemy : MonoBehaviour
    {
        [SerializeField] private float _speed;
        
        private Queue<Vector3> _wayPoints = new Queue<Vector3>();
        private EnemiesGenerator _generator;
        
        public void Construct(EnemiesGenerator generator)
        {
            _generator = generator;
        }
        
        public void SetupWaypoints(Vector3 XYWaypoint)
        {
            _wayPoints.Enqueue(XYWaypoint);
        }

        private void Update()
        {
            bool IsDestinationNear()
            {
                const float CriticalDistance = 20;
                return (_wayPoints.Peek() - transform.position).sqrMagnitude < CriticalDistance;
            }
            
            if (_wayPoints.Count == 0)
            {
                _generator.DealDamage(Time.deltaTime * 5);
                return;
            }

            Vector3 waypoint = _wayPoints.Peek();
            Vector3 position = transform.position;
            position = Vector3.MoveTowards(position,
                new Vector3(waypoint.x, waypoint.y, position.z), Time.deltaTime * _speed);
            transform.position = position;

            if (IsDestinationNear())
            {
                _wayPoints.Dequeue();
            }
        }

        private int _counter = 4;
        
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
            {
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
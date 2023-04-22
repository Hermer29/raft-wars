using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurretMinigame
{
    public class TurretMinigameEnemy : MonoBehaviour
    {
        [SerializeField] private float _speed;
        
        private Queue<Vector3> _wayPoints = new Queue<Vector3>();
        
        public void SetupWaypoints(Vector3 ZYWaypoint)
        {
            _wayPoints.Enqueue(ZYWaypoint);
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
                //TODO: Наносим игроку урон
                return;
            }

            Vector3 waypoint = _wayPoints.Peek();
            Vector3 position = transform.position;
            position = Vector3.MoveTowards(position,
                new Vector3(position.x, waypoint.y, waypoint.z), Time.deltaTime * _speed);
            transform.position = position;

            if (IsDestinationNear())
            {
                _wayPoints.Dequeue();
            }
        }
    }
}
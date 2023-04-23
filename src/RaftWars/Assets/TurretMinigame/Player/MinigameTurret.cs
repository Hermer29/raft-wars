using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace TurretMinigame.Player
{
    public class MinigameTurret : MonoBehaviour
    {
        private TurretMinigameFactory _factory;
        private Coroutine _shootingProcess;
        
        [SerializeField] private Transform _tower;
        [SerializeField] private Vector3 _shootDirection;
        [SerializeField] private Transform _riflesEnd;
        [SerializeField] private float _timeToFlyMeter;
        
        private const float AttackCooldown = .3f;
        private const float Sensitivity = 1f;

        public void Construct(TurretMinigameFactory factory)
        {
            _factory = factory;
        }
        
        public void StartShooting()
        {
            _shootingProcess = StartCoroutine(ShootOverTime());
        }

        public void StopShooting()
        {
            StopCoroutine(_shootingProcess);
        }

        private IEnumerator ShootOverTime()
        {
            while (true)
            {
                yield return new WaitForSeconds(AttackCooldown);
                GameObject bullet = _factory.CreateBullet();
                bullet.transform.position = _riflesEnd.position;
                bullet.transform.forward = _riflesEnd.transform.forward;
                bullet.transform.DOMove(
                    bullet.transform.position + bullet.transform.forward,
                    _timeToFlyMeter).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
                
            }
        }

        private IEnumerator WaitForSomeTime(GameObject bullet)
        {
            yield return new WaitForSeconds(2f);
            _factory.FreeBullet(bullet);
        }

        public void Rotate(float delta)
        {
            float rotationTowards = _tower.rotation.eulerAngles.y + delta;
            _tower.rotation = Quaternion.Euler(0, 
                Mathf.MoveTowardsAngle(
                    _tower.rotation.eulerAngles.y, 
                    rotationTowards, 
                    Sensitivity), 0);
        }

        private void OnDrawGizmos()
        {
            Vector3 riflesEndPosition = _riflesEnd.position + _shootDirection;
            Gizmos.DrawLine(_riflesEnd.position, riflesEndPosition.normalized);
        }
    }
}
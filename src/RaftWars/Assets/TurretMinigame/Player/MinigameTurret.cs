using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TurretMinigame.Player
{
    public class MinigameTurret : MonoBehaviour
    {
        private TurretMinigameFactory _factory;
        private Coroutine _shootingProcess;
        private Dictionary<GameObject, Coroutine> _queue = new Dictionary<GameObject, Coroutine>();

        [SerializeField] private Transform _tower;
        [SerializeField] private Vector3 _shootDirection;
        [SerializeField] private Transform _riflesEnd;
        [SerializeField] private float _timeToFlyMeter;
        [SerializeField] private ParticleSystem _lostParticles;
        [SerializeField] private float _attackFrequency = .25f;
        public Sprite Illustration;
        private AudioService _audio;

        private const float Sensitivity = 1f;

        public void Construct(TurretMinigameFactory factory, AudioService audio)
        {
            _factory = factory;
            _audio = audio;
        }
        
        public void StartShooting()
        {
            _shootingProcess = StartCoroutine(ShootOverTime());
        }

        public void StopShooting()
        {
            StopCoroutine(_shootingProcess);
        }

        public void BreakTower()
        {
            _tower.gameObject.SetActive(false);
            _lostParticles.gameObject.SetActive(true);
            _lostParticles.Play(true);
        }

        private IEnumerator ShootOverTime()
        {
            while (true)
            {
                yield return new WaitForSeconds(_attackFrequency);
                _audio.PlayShotSound();
                GameObject bullet = _factory.CreateBullet();
                var trailRenderer = bullet.GetComponent<TrailRenderer>();
                float prevTime = trailRenderer.time;
                trailRenderer.time = 0;
                bullet.transform.position = _riflesEnd.position;
                bullet.transform.forward = _riflesEnd.transform.forward;
                yield return null;
                trailRenderer.time = prevTime;
                bullet.transform.DOMove(bullet.transform.position + bullet.transform.forward, _timeToFlyMeter)
                    .SetLoops(-1, LoopType.Incremental)
                    .SetEase(Ease.Linear);
                if(_queue.ContainsKey(bullet) == false)
                    _queue.Add(bullet, null);
                _queue[bullet] = StartCoroutine(WaitForSomeTime(bullet));
            }
        }

        private IEnumerator WaitForSomeTime(GameObject bullet)
        {
            yield return new WaitForSeconds(2f);
            ReturnBullet(bullet);
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

        public void ReturnBullet(GameObject bullet)
        {
            StopCoroutine(_queue[bullet]);
            if (_queue[bullet] != null)
            {
                _queue[bullet] = null;
            }
            _factory.FreeBullet(bullet);
            bullet.transform.DOKill();
            bullet.transform.position = _riflesEnd.transform.position;
        }
    }
}
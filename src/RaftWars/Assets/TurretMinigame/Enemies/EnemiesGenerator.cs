using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TurretMinigame
{
    public class EnemiesGenerator : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private float _spawnFrequency;
        [SerializeField] private Vector2 _zScatter;
        [SerializeField] private GameObject _groupPrefab;
        [SerializeField] private Vector3[] _wayPoints;
        
        private Coroutine _generation;
        private int _enemyCount;
        private int _counterToSpawn = 5;
        private float _playerHp = 10;

        public event Action PlayerWon;
        public event Action PlayerLost;

        public void StartGeneration()
        {
            _generation = StartCoroutine(Generate());
        }
        
        public IEnumerator Generate()
        {
            while (true)
            {
                if (_counterToSpawn == 0)
                    yield break;
                _counterToSpawn--;
                yield return new WaitForSeconds(_spawnFrequency);
                Vector3 position = _spawnPoint.position;
                float zPoint = Random.Range(_zScatter.x, _zScatter.y);
                float point = position.z + zPoint;
                position.z = point;
                var instantiated = Instantiate(_groupPrefab, position, Quaternion.identity);
                foreach (TurretMinigameEnemy turretMinigameEnemy in instantiated.GetComponentsInChildren<TurretMinigameEnemy>())
                {
                    turretMinigameEnemy.Construct(this);
                    foreach (Vector3 wayPoint in _wayPoints)
                    {
                        turretMinigameEnemy.SetupWaypoints(wayPoint);
                    }
                }
            }
        }

        public void DealDamage(float damage)
        {
            _playerHp -= damage;
            if (_playerHp <= 0)
            {
                PlayerLost?.Invoke();
            }
        }

        private void OnDrawGizmos()
        {
            for (int i = 0, j = 1; j < _wayPoints.Length; i++, j++)
            {
                Gizmos.DrawLine(_wayPoints[i], _wayPoints[j]);
            }
        }

        public void OneDied()
        {
            _enemyCount--;
            if (_enemyCount == 0)
            {
                PlayerWon?.Invoke();
            }
        }
    }
}
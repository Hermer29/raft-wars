using System;
using System.Collections;
using TurretMinigame.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TurretMinigame.Enemies
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
        private float _playerHp = MaxPlayerHealth;
        private MinigameTurret _minigameTurret;
        private PlayerEnemiesView _view;
        private const int EnemiesAmount = 9 * 5;
        private int _enemiesRemaining = EnemiesAmount;
        private bool _ended;

        private const float MaxPlayerHealth = 10;

        public event Action PlayerWon;
        public event Action PlayerLost;
        public int KillCount => EnemiesAmount - _enemiesRemaining;
        public bool GameEnded => _ended;
        public float Completion => (float) _enemiesRemaining / EnemiesAmount;

        public void Construct(MinigameTurret minigameTurret, PlayerEnemiesView view)
        {
            _view = view;
            _minigameTurret = minigameTurret;
        }
        
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
                GameObject instantiated = Instantiate(_groupPrefab, position, Quaternion.identity);
                foreach (TurretMinigameEnemy turretMinigameEnemy in instantiated.GetComponentsInChildren<TurretMinigameEnemy>())
                {
                    turretMinigameEnemy.Construct(this, _minigameTurret);
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
                _ended = true;
            }
            _view.ShowHealth(_playerHp / MaxPlayerHealth);
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
            _enemiesRemaining--;
            if (_enemiesRemaining == 0)
            {
                PlayerWon?.Invoke();
                _ended = true;
            }
            _view.ShowEnemies(Completion);
        }
    }
}
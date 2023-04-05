using System.Linq;
using UnityEngine;

namespace InputSystem
{
    public class PlayerService
    {
        private Player _player;
        private Enemy _enemyInExclusionZone;

        public PlayerService(Player player)
        {
            _player = player;
        }

        public float PlayerStatsSum => _player.damage + _player.hp;

        public Vector3 Position => _player.transform.position;
        public bool GameStarted => _player.canPlay;
        public bool IsDead => _player.isDead;
        public bool InBattle => _player.battle;
        public bool ExistsEnemyThatAlreadyInExclusionZone => _enemyInExclusionZone != null;
        public Enemy EnemyInExclusionZone => _enemyInExclusionZone;

        public void AddPeople()
        {
            var peoplePrefab = Resources.Load<GameObject>("Prefabs/People");
            var platform = _player.GetPlatforms().First();
            var position = Platform.FindPointOnPlatform(platform.transform.position);
            var instance = GameObject.Instantiate(peoplePrefab, position, Quaternion.identity, platform.transform);
            _player.AddPeople(instance.GetComponent<People>());
            instance.GetComponent<People>().SetRelatedPlatform(platform.GetComponent<Platform>());
        }

        public void RegisterAsEnemyInExclusionZone(Enemy enemy)
        {
            _enemyInExclusionZone = enemy;
        }

        public void UnregisterEnemyInExclusionZone()
        {
            _enemyInExclusionZone = null;
        }

        public void DoubleSpeed()
        {
            _player.speed *= 2;
        }
    }
}
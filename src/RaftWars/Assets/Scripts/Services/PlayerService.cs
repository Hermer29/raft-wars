using System.Linq;
using Skins;
using UnityEngine;

namespace InputSystem
{
    public class PlayerService
    {
        public Player PlayerInstance;
        private Enemy _enemyInExclusionZone;

        public PlayerService(Player playerInstance)
        {
            PlayerInstance = playerInstance;
        }

        public float PlayerStatsSum => PlayerInstance.damage + PlayerInstance.hp;

        public Vector3 Position => PlayerInstance.transform.position;
        public bool GameStarted => PlayerInstance.canPlay;
        public bool IsDead => PlayerInstance.isDead;
        public bool InBattle => PlayerInstance.battle;
        public bool ExistsEnemyThatAlreadyInExclusionZone => _enemyInExclusionZone != null;
        public Enemy EnemyInExclusionZone => _enemyInExclusionZone;
        public Vector3 MoveDirection => PlayerInstance.MoveDirection;

        public void AddPeople()
        {
            var peoplePrefab = Resources.Load<GameObject>("Prefabs/People");
            var platform = PlayerInstance.GetPlatforms().First();
            var position = Platform.FindPointOnPlatform(platform.transform.position);
            var instance = GameObject.Instantiate(peoplePrefab, position, Quaternion.identity, platform.transform);
            PlayerInstance.AddPeople(instance.GetComponent<People>());
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
            PlayerInstance.speed *= 2;
        }

        public void RepaintWith(PlayerColors material)
        {
            PlayerInstance.RepaintWith(material);
        }
    }
}
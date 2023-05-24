using System.Linq;
using RaftWars.Pickables;
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
        public Vector3 MoveDirectionXZ => PlayerInstance.MoveDirectionXZ;

        public void AddPeople()
        {
            var peoplePrefab = Resources.Load<GameObject>("Prefabs/People");
            var isFound = PlayerInstance.TryFindNotFullPlatform(out var platform);
            if (isFound == false)
                return;
            platform.TryTakePeople(peoplePrefab);
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

        public void DealDamage()
        {
            PlayerInstance.DealDamage();
        }

        public void Amplify(int stats)
        {
            PlayerInstance.Amplify(stats);
        }

        public void Dispose()
        {
            Object.Destroy(PlayerInstance._camera.gameObject);
            Object.Destroy(PlayerInstance.gameObject);
        }

        public void Revive()
        {
            PlayerInstance.Revive();
            AddPeople();
            AddPeople();
        }

        public void AddPlatform(GameObject selectedPlatformPlatform)
        {
            PlayerInstance.GetAnotherPlatform().TakePlatform(selectedPlatformPlatform, Vector3.zero);
        }
    }
}
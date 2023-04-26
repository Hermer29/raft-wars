using System.Collections;
using Common;
using RaftWars.Infrastructure.AssetManagement;
using UnityEngine;

namespace Units
{
    public class BarracksSpawner : MonoBehaviour
    {
        private FighterRaft _raft;
        private int _spawnLimit;
        private GameObject _peoplePrefab;
        private SpecialPlatforms.Concrete.Barracks _balanceData;
        private Coroutine _barracksSpawning;

        public void Construct(FighterRaft raft, SpecialPlatforms.Concrete.Barracks balanceData, bool useDefaultValues)
        {
            _raft = raft;
            _balanceData = balanceData;
            _spawnLimit = useDefaultValues ? balanceData.SpawnPeopleDefaultLimit : balanceData.SpawnPeopleLimit;
            _peoplePrefab = AssetLoader.LoadPeople().gameObject;
        }

        private void Start()
        {
            _barracksSpawning = StartCoroutine(CreateNewPeopleOverTime());
        }

        public void StopSpawning()
        {
            if(_barracksSpawning == null)
                StopCoroutine(_barracksSpawning);
        }
        
        private IEnumerator CreateNewPeopleOverTime()
        {
            while (_spawnLimit > 0)
            {
                yield return new WaitWhile(() => TryCreateNewPeople() == false);
                _spawnLimit--;
                yield return new WaitForSeconds(_balanceData.SpawnPeopleTime);
            }
        }

        private bool TryCreateNewPeople()
        {
            if (_raft.TryGetNotFullPlatform(out Platform platform) == false)
                return false;

            return platform.TryTakePeople(
                warriorPrefab: _peoplePrefab,
                specifiedSpawnPoint: transform.position);
        }
    }
}
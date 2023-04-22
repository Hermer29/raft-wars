using System.Collections;
using Infrastructure.States;
using TurretMinigame;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RaftWars.Infrastructure
{
    public class LoadLevelState : IPayloadedState<int>
    {
        private readonly StateMachine _stateMachine;
        private readonly ICoroutineRunner _coroutines;
        private LoadingScreen _loading;
        private readonly GameFactory _factory;
        
        private int _level;
        private const int GameplaySceneIndex = 1;

        public LoadLevelState(StateMachine stateMachine, ICoroutineRunner coroutines, LoadingScreen loading)
        {
            _loading = loading;
            _stateMachine = stateMachine;
            _coroutines = coroutines;
            _factory = new GameFactory(_coroutines);
        }
        
        public void Enter(int level)
        {
            _level = level;
            _coroutines.StartCoroutine(Load());
        }

        public void Exit()
        {
            
        }

        private IEnumerator Load()
        {
            const float totalOperations = 3;

            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GameplaySceneIndex);
            while (asyncOperation.isDone == false)
            {
                _loading.SetSliderProcess(asyncOperation.progress / totalOperations);
                yield return null;
            }

            float operationPercent = asyncOperation.progress;

            var loadingLevelAssets = _factory.CreateMapGenerator(_level);
            while (loadingLevelAssets.IsDone == false)
            {
                _loading.SetSliderProcess((loadingLevelAssets.PercentComplete + operationPercent) / totalOperations);
                yield return null;
            }
            Game.MapGenerator = GameObject.Instantiate(loadingLevelAssets.Result).GetComponent<MapGenerator>();
            _loading.SetSliderProcess(2f / 3f);
            _stateMachine.Enter<CreateServicesState>();
            //_stateMachine.Enter<TurretMinigameState>();
        }
    }
}
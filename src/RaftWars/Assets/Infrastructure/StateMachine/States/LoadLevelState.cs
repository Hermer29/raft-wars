using System.Collections;
using UnityEngine.SceneManagement;

namespace RaftWars.Infrastructure
{
    public class LoadLevelState : IPayloadedState<int>
    {
        private readonly StateMachine _stateMachine;
        private readonly ICoroutineRunner _coroutines;
        private LoadingScreen _loading;
        
        private const int GameplayScene = 1;

        public LoadLevelState(StateMachine stateMachine, ICoroutineRunner coroutines, LoadingScreen loading)
        {
            _loading = loading;
            _stateMachine = stateMachine;
            _coroutines = coroutines;
        }
        
        public void Enter(int level)
        {
            Game.MapGenerator = GameFactory.CreateMapGenerator(level);
            _coroutines.StartCoroutine(Load());
        }

        public void Exit()
        {
            
        }

        private IEnumerator Load()
        {
            var asyncOperation = SceneManager.LoadSceneAsync(GameplayScene);
            while (asyncOperation.isDone == false)
            {
                _loading.SetSliderProcess(asyncOperation.progress);
                yield return null;
            }
            _loading.SetSliderProcess(.7f);
            
            _stateMachine.Enter<CreateServicesState>();
        }
    }
}
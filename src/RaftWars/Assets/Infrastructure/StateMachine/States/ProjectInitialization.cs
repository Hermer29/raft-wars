using System.Collections;
using UnityEngine;
using Agava.YandexGames;
using RaftWars.Infrastructure;
using RaftWars.Infrastructure.AssetManagement;
using RaftWars.Infrastructure.Services;

namespace Infrastructure.States
{
    public class ProjectInitialization : IState
    {
        private static CrossLevelServices _crossLevelServices;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly StateMachine _stateMachine;
        
        private bool _initializationCompleted;

        public ProjectInitialization(ICoroutineRunner coroutineRunner, StateMachine stateMachine)
        {
            _coroutineRunner = coroutineRunner;
            _stateMachine = stateMachine;
            
            YandexGamesSdk.CallbackLogging = true;
        }
        
        public void Exit()
        {
            
        }
        
        public void Enter()
        {
            Game.FeatureFlags = AssetLoader.LoadFeatureFlags();
            if(Game.FeatureFlags.InitializeYandexGames && !YandexGamesSdk.IsInitialized)
            {
                _coroutineRunner.StartCoroutine(WaitForSdkInitialization());
                return;
            }
            ContinueInitialization();
        }

        private IEnumerator WaitForSdkInitialization()
        {
            _coroutineRunner.StartCoroutine(PollInitializationCompletion());
            yield return _coroutineRunner.StartCoroutine(
                YandexGamesSdk.Initialize(() => _initializationCompleted = true));
        }

        private IEnumerator PollInitializationCompletion()
        {
            yield return new WaitWhile(() => _initializationCompleted == false);
            ContinueInitialization();
        }

        private void ContinueInitialization()
        {
            SelectPrefsImplementation(
                dataLoadingContinuation: ContinueServicesCreation);
        }

        private void SelectPrefsImplementation(System.Action dataLoadingContinuation)
        {
            switch(Game.FeatureFlags.PrefsImplementation)
            {
                case PrefsOptions.YandexCloud:
                    CrossLevelServices.PrefsService = new YandexPrefsService(_coroutineRunner, dataLoadingContinuation);
                    break;
                case PrefsOptions.PlayerPrefs:
                    CrossLevelServices.PrefsService = new PlayerPrefsService(_coroutineRunner);
                    dataLoadingContinuation.Invoke();
                    break;
            }
        }

        private void ContinueServicesCreation()
        {
            CrossLevelServices.LevelService = new LevelService(CrossLevelServices.PrefsService);
            AllServices.Register<IPrefsService>(CrossLevelServices.PrefsService);
            AllServices.Register<LevelService>(CrossLevelServices.LevelService);
            _stateMachine.Enter<BootstrapState>();
        }
    }
}
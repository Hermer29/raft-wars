using System;
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
            if (Application.isEditor == false)
            {
                if(Game.FeatureFlags.InitializeYandexGames && !YandexGamesSdk.IsInitialized)
                {
                    _coroutineRunner.StartCoroutine(WaitForSdkInitialization());
                    return;
                }
            }
            Debug.Log("Not initializing yandex sdk, because we're not in build");
            SelectPrefsImplementation();
            ContinueServicesCreation();
        }

        private IEnumerator WaitForSdkInitialization()
        {
            yield return YandexGamesSdk.Initialize();
            yield return new WaitForSeconds(1f);
            SelectPrefsImplementation();
            
            yield return new WaitWhile(() => CrossLevelServices.PrefsService.IsDataLoaded);
            ContinueServicesCreation();
        }

        private void SelectPrefsImplementation()
        {
            if (Application.isEditor)
            {
                SelectPlayerPrefs();
                return;
            }
            
            switch(Game.FeatureFlags.PrefsImplementation)
            {
                case PrefsOptions.YandexCloud:
                    CrossLevelServices.PrefsService = new YandexPrefsService(_coroutineRunner);
                    break;
                case PrefsOptions.PlayerPrefs:
                    SelectPlayerPrefs();
                    break;
            }
        }

        private void SelectPlayerPrefs()
        {
            Debug.Log("Selected player prefs implementation because we're not in build");
            CrossLevelServices.PrefsService = new PlayerPrefsService(_coroutineRunner);
        }

        private void ContinueServicesCreation()
        {
            CrossLevelServices.LevelService = new LevelService(CrossLevelServices.PrefsService);
            AllServices.Register(CrossLevelServices.PrefsService);
            AllServices.Register(CrossLevelServices.LevelService);
            _stateMachine.Enter<BootstrapState>();
        }
    }
}
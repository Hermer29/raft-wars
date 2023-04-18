﻿using RaftWars.Infrastructure.AssetManagement;
using RaftWars.Infrastructure.Services;
using Agava.YandexGames;

namespace RaftWars.Infrastructure
{
    public class ProjectInitialization : IState
    {
        private static CrossLevelServices _crossLevelServices;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly StateMachine _stateMachine;

        public ProjectInitialization(ICoroutineRunner coroutineRunner, StateMachine stateMachine)
        {
            _coroutineRunner = coroutineRunner;
            _stateMachine = stateMachine;
        }
        
        public void Exit()
        {
            
        }
        
        public void Enter()
        {
            Game.FeatureFlags = AssetLoader.LoadFeatureFlags();
            if(Game.FeatureFlags.InitializeYandexGames)
            {
                YandexGamesSdk.Initialize(onSuccessCallback: () => {
                    ContinueInitialization();
                });
                return;
            }
           ContinueInitialization();
        }

        private void ContinueInitialization()
        {
            SelectPrefsImplementation();
            _stateMachine.Enter<BootstrapState>();
        }

        private void SelectPrefsImplementation()
        {
            switch(Game.FeatureFlags.PrefsImplementation)
            {
                case PrefsOptions.YandexCloud:
                    CrossLevelServices.PrefsService = new YandexPrefsService(_coroutineRunner);
                    break;
                case PrefsOptions.PlayerPrefs:
                    CrossLevelServices.PrefsService = new PlayerPrefsService(_coroutineRunner);
                    break;
            }
            CrossLevelServices.LevelService = new LevelService(CrossLevelServices.PrefsService);
        }
    }
}
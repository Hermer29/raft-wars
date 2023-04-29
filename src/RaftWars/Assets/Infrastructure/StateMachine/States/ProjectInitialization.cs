using System.Collections;
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
            if(Game.FeatureFlags.InitializeYandexGames)
            {
                if (!YandexGamesSdk.IsInitialized)
                {
                    _coroutineRunner.StartCoroutine(YandexGamesSdk.Initialize(ContinueInitialization));
                    return;
                }
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
            AllServices.Register<IPrefsService>(CrossLevelServices.PrefsService);
            AllServices.Register<LevelService>(CrossLevelServices.LevelService);
        }
    }
}
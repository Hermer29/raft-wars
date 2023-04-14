
using RaftWars.Infrastructure.AssetManagement;
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
            _crossLevelServices ??= new CrossLevelServices(_coroutineRunner, Game.FeatureFlags);
            if(Game.FeatureFlags.InitializeYandexGames)
            {
                Agava.YandexGames.YandexGamesSdk.Initialize(onSuccessCallback: () => {
                    _stateMachine.Enter<BootstrapState>();
                });
                return;
            }
            _stateMachine.Enter<BootstrapState>();
        }
    }
}
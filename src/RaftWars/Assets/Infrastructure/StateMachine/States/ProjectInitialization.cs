namespace RaftWars.Infrastructure
{
    public class ProjectInitialization : IState
    {
        private static CrossLevelServices _crossLevelServices;
        private readonly ICoroutineRunner _coroutineRunner;

        public ProjectInitialization(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }
        
        public void Exit()
        {
            
        }
        
        public void Enter()
        {
            _crossLevelServices ??= new CrossLevelServices(_coroutineRunner);
        }
    }
}
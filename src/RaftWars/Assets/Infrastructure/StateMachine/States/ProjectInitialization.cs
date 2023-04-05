namespace RaftWars.Infrastructure
{
    public class ProjectInitialization : IState
    {
        private static CrossLevelServices _crossLevelServices;
        
        public void Exit()
        {
            
        }
        
        public void Enter()
        {
            _crossLevelServices ??= new CrossLevelServices();
        }
    }
}
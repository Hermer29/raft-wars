using RaftWars.Infrastructure;

namespace Infrastructure.States
{
    public class CreateSpecialPlatformsWindow : IState
    {
        private readonly StateMachine _stateMachine;

        public CreateSpecialPlatformsWindow(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public void Exit()
        {
            
        }

        public void Enter()
        {
            
        }
    }
}
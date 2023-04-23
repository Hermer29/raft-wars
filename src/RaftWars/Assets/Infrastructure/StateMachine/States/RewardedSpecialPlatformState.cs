using Infrastructure.States;

namespace RaftWars.Infrastructure
{
    public class RewardedSpecialPlatformState : IState
    {
        private readonly StateMachine _stateMachine;

        public RewardedSpecialPlatformState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public void Exit()
        {
            Game.GameManager.DestroyAll();
        }

        public void Enter()
        {
            _stateMachine.Enter<TurretMinigameState>();
        }
    }
}
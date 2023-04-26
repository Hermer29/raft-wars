using RaftWars.Infrastructure;

namespace Infrastructure.States
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
            Game.Hud.gameObject.SetActive(false);
        }

        public void Enter()
        {
            _stateMachine.Enter<TurretMinigameState>();
        }
    }
}
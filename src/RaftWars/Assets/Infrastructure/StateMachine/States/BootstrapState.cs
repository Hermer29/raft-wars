using RaftWars.Infrastructure;
using UnityEngine.SceneManagement;

namespace Infrastructure.States
{
    public class BootstrapState : IState
    {
        private readonly StateMachine _stateMachine;
        private const string BootSceneName = "Bootstrap";

        public BootstrapState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public void Enter()
        {
            if (SceneManager.GetActiveScene().name != BootSceneName)
            {
                SceneManager.LoadScene(BootSceneName);
                return;
            }
            _stateMachine.Enter<CreateServicesState>();
        }

        public void Exit()
        {
        }
    }
}
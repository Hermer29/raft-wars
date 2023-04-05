using UnityEngine;
using UnityEngine.SceneManagement;

namespace RaftWars.Infrastructure
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
            }
            _stateMachine.Enter<LoadLevelState, int>(
                Mathf.Clamp(CrossLevelServices.LevelService.Level, 1, 999));
        }

        public void Exit()
        {
        }
    }
}
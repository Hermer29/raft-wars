namespace RaftWars.Infrastructure
{
    public class CreateIMGUIState : IState
    {
        public void Exit()
        {
            
        }

        public void Enter()
        {
            if (Game.FeatureFlags.IMGUIEnabled)
            { 
                GameFactory.CreateIMGUI();
            }
        }
    }
}
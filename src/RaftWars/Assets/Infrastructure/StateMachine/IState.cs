namespace RaftWars.Infrastructure
{
    public interface IState : IExitableState
    {
        void Enter();
    }
}
namespace RaftWars.Infrastructure
{
    public interface IPayloadedState<TPayload> : IExitableState
    {
        public void Enter(TPayload material);
    }
}
using System;
using System.Collections.Generic;
using Infrastructure;
using Infrastructure.States;

namespace RaftWars.Infrastructure
{
    public class StateMachine
    {
        private Dictionary<Type, IExitableState> _states;
        private IExitableState _activeState;
        
        public StateMachine(ICoroutineRunner coroutineRunner, LoadingScreen loadingScreen)
        {
            _states = new Dictionary<Type, IExitableState>
            {
                [typeof(BootstrapState)] = new BootstrapState(this),
                [typeof(LoadLevelState)] = new LoadLevelState(this, coroutineRunner, loadingScreen),
                [typeof(GameplayState)] = new GameplayState(this, loadingScreen, coroutineRunner),
                [typeof(ProjectInitialization)] = new ProjectInitialization(coroutineRunner, this),
                [typeof(TurretMinigameState)] = new TurretMinigameState(this, loadingScreen, Game.AdverisingService),
                [typeof(RewardedSpecialPlatformState)] = new RewardedSpecialPlatformState(this),
                [typeof(CreateServicesState)] = new CreateServicesState(this, coroutineRunner)
            };
        }

        public void Enter<TState>() where TState: class, IState
        {
            var state = ChangeState<TState>();
            state.Enter();
        }
        
        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            var state = ChangeState<TState>();
            state.Enter(payload);
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _activeState?.Exit();
            IExitableState state = GetState<TState>();
            _activeState = state;
            return (TState) state;
        }

        private IExitableState GetState<TState>() where TState: class, IExitableState
        {
            return _states[typeof(TState)] as TState;
        }
    }
}
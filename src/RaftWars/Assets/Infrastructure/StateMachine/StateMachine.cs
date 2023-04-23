using System;
using System.Collections.Generic;
using Infrastructure.States;
using InputSystem;
using Services;

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
                [typeof(CreateServicesState)] = new CreateServicesState(this, loadingScreen, coroutineRunner),
                [typeof(ProjectInitialization)] = new ProjectInitialization(coroutineRunner, this),
                [typeof(TurretMinigameState)] = new TurretMinigameState(this, loadingScreen)
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
using System.Collections.Generic;
using Infrastructure.Platforms;
using LanguageChanger;
using RaftWars.Infrastructure;
using RaftWars.Infrastructure.AssetManagement;
using RaftWars.Infrastructure.Services;
using SpecialPlatforms;
using UnityEngine;

namespace Infrastructure.States
{
    public class CreateServicesState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly ICoroutineRunner _runner;

        public CreateServicesState(StateMachine stateMachine, ICoroutineRunner runner)
        {
            _stateMachine = stateMachine;
            _runner = runner;
        }
        
        public void Exit()
        {
        }

        public void Enter()
        {
            AllServices.Register<ICoroutineRunner>(_runner);
            AllServices.Register<LocalizationService>(Object.FindObjectOfType<LocalizationService>());
            AllServices.Register<SaveService>(new SaveService(_runner, CrossLevelServices.PrefsService));
            AllServices.Register<PlatformsLoader>(new PlatformsLoader());
            AllServices.Register<PlatformsFactory>(new PlatformsFactory(
                AllServices.GetSingle<SaveService>(),
                AllServices.GetSingle<PlatformsLoader>()));
            AllServices.Register<IEnumerable<SpecialPlatform>>(
                AllServices.GetSingle<PlatformsFactory>().CreatePlatforms());
            new Game(_stateMachine);
            var owningSequence = new OwningSequence<SpecialPlatform>(Game.PropertyService, Game.FeatureFlags);
            AllServices.Register<OwningSequence<SpecialPlatform>>(owningSequence);
            _stateMachine.Enter<LoadLevelState, int>(
                Mathf.Clamp(CrossLevelServices.LevelService.Level, 1, 999));
        }
    }
}
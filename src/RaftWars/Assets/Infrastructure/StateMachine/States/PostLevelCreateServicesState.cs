﻿using DefaultNamespace;
using RaftWars.Infrastructure;
using UnityEngine;

namespace Infrastructure.States
{
    public class PostLevelCreateServicesState : IPayloadedState<MapGenerator>
    {
        private readonly StateMachine _stateMachine;

        public PostLevelCreateServicesState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public void Exit()
        {
            
        }

        public void Enter(MapGenerator mapGenerator)
        {
            Game.AudioService = GameFactory.CreateAudioService();
            Game.MapGenerator = Object.Instantiate(mapGenerator);
            bool previousAudioState = true;
            
            Game.AdverisingService.AdvertisingStarted += () =>
            {
                previousAudioState = Game.AudioService.State;
                Game.AudioService.SetState(false);
            };
            Game.AdverisingService.AdvertisingEnded += () => Game.AudioService.SetState(previousAudioState);
            
            switch (Game.FeatureFlags.SkipTo)
            {
                case SkipTo.Gameplay:
                    _stateMachine.Enter<TurretMinigameState>();
                    break;
                case SkipTo.LevelRewards:
                    _stateMachine.Enter<RewardedSpecialPlatformState>();
                    break;
                case SkipTo.NoSkipping:
                    _stateMachine.Enter<GameplayState>();
                    break;
            }
        }
    }
}
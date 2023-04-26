using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using InputSystem;
using RaftWars.Infrastructure;
using RaftWars.Infrastructure.Services;
using Services;
using SpecialPlatforms;

namespace Infrastructure.States
{
    public class RewardedSpecialPlatformState : IState
    {
        private readonly StateMachine _stateMachine;
        private AdvertisingService _ads;
        private PropertyService _props;
        private IEnumerable<SpecialPlatform> _specialPlatforms;
        private SpecialPlatform _specialOne;

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
            _ads = AllServices.GetSingle<AdvertisingService>();
            _props = AllServices.GetSingle<PropertyService>();
            _specialPlatforms = AllServices.GetSingle<IEnumerable<SpecialPlatform>>();

            _specialOne =
                _specialPlatforms.FirstOrDefault(x => 
                    x.RequiredLevel == CrossLevelServices.LevelService.Level && _props.IsOwned(x) == false);
            if (_specialOne == null)
            {
                Continue();
                return;
            }
            var window = GameFactory.CreateSPRewardWindow();
            window.Claim.onClick.AddListener(WatchAd);
            window.NotClaim.onClick.AddListener(Continue);
            window.ShowSpecialPlatform(_specialOne.SpRewardIllustration, _specialOne);
        }

        private void WatchAd()
        {
            _ads.ShowRewarded(() =>
            {
                _props.Own(_specialOne);
                Continue();
            });
        }

        private void Continue()
        {
            _stateMachine.Enter<TurretMinigameState>();
        }
    }
}
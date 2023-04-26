using System.Collections.Generic;
using System.Linq;
using InputSystem;
using RaftWars.Infrastructure;
using RaftWars.Infrastructure.Services;
using Services;
using SpecialPlatforms;
using SpecialPlatforms.SPRewardState;
using Object = UnityEngine.Object;

namespace Infrastructure.States
{
    public class RewardedSpecialPlatformState : IState
    {
        private readonly StateMachine _stateMachine;
        private AdvertisingService _ads;
        private PropertyService _props;
        private IEnumerable<SpecialPlatform> _specialPlatforms;
        private SpecialPlatform _specialOne;
        private SPRewardWindow _window;

        public RewardedSpecialPlatformState(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public void Exit()
        {
            Game.GameManager.DestroyAll();
            Object.Destroy(_window);
        }

        public void Enter()
        {
            Game.Hud.gameObject.SetActive(false);
            _ads = Game.AdverisingService;
            _props = Game.PropertyService;
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
            _window = window;
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
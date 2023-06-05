using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DefaultNamespace.Skins;
using InputSystem;
using Interface.RewardWindows;
using RaftWars.Infrastructure;
using Services;
using Object = UnityEngine.Object;

namespace SpecialPlatforms
{
    public class RewardWindowProcessing
    {
        private readonly IEnumerable<IShopProduct> _shopProducts;
        private readonly PropertyService _propertyService;
        private readonly AdvertisingService _advertisingService;
        private readonly PlayerUsingService _usingService;
        private IShopProduct _shownOne;
        private RandomRewardWindow _window;

        public RewardWindowProcessing(IEnumerable<IShopProduct> shopProducts, PropertyService propertyService, 
            AdvertisingService advertisingService, PlayerUsingService usingService)
        {
            _shopProducts = shopProducts;
            _propertyService = propertyService;
            _advertisingService = advertisingService;
            _usingService = usingService;

            Setup();
        }
        
        public bool ContinuationCalled { get; private set; }
        
        public event Action Continuation;

        private void Setup()
        {
            var possible = _shopProducts
                .Where(x => _propertyService.IsOwned(x) == false);
            var shopProducts = possible as IShopProduct[] ?? possible.ToArray();
            if (shopProducts.Any() == false)
            {
                ContinuationCalled = true;
                Continuation?.Invoke();
                return;
            }

            _shownOne = shopProducts.Random();

            _window = GameFactory.CreateRewardWindow();
            _window.Show(_shownOne);
            
            _window.Decline.onClick.AddListener(OnDecline);
            _window.Accept.onClick.AddListener(OnAccept);
        }

        private void OnDecline()
        {
            DestroyWindow();
           
        }

        private void OnAccept()
        {
            _advertisingService.ShowRewarded(() =>
            {
                _propertyService.Own(_shownOne);
                _usingService.Use(_shownOne);
                DestroyWindow();
            });
        }

        private void DestroyWindow()
        {
            ContinuationCalled = true;
            Continuation?.Invoke();
            Object.Destroy(_window.gameObject);
        }
    }
}
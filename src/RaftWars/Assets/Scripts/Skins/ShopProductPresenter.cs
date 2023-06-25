using System;
using DefaultNamespace.Skins;
using Infrastructure;
using InputSystem;
using Services;
using RaftWars.Infrastructure;

namespace Skins
{
    public class ShopProductPresenter
    {
        private readonly ShopEntry _shopEntry;
        private readonly IShopProduct _product;
        private readonly YandexIAPService _iapService;
        private readonly PlayerMoneyService _moneyService;
        private readonly PlayerUsingService _usingService;
        private readonly PropertyService _propertyService;

        public event Action PlayerTriedToBuyAndInssufficientFunds;
        public event Action PlayerTriedToBuyAndIapRaisedError;
        
        public ShopProductPresenter(ShopEntry shopEntry, IShopProduct product, YandexIAPService iapService, PlayerMoneyService moneyService,
            PlayerUsingService usingService, PropertyService propertyService)
        {
            _shopEntry = shopEntry;
            _product = product;
            _iapService = iapService;
            _moneyService = moneyService;
            _usingService = usingService;
            _propertyService = propertyService;

            _shopEntry.Use?.onClick.AddListener(OnUseClicked);
            _shopEntry.BuyForCoins?.onClick.AddListener(OnBuyWithCoins);
            _shopEntry.BuyForYans?.onClick.AddListener(OnBuyWithYans);
            _shopEntry.Reward?.onClick.AddListener(OnGetWithReward);

            if (propertyService.IsOwned(_product))
            {
                if (_usingService.IsSavedUsed(_product))
                    shopEntry.SetEntryState(ShopEntry.EntryState.selected);
                
                else
                    shopEntry.SetEntryState(ShopEntry.EntryState.owned);
                
            }
            else
            {
                shopEntry.SetEntryState(ShopEntry.EntryState.closed);
            }

            _propertyService.PropertyOwned += UpdateRequested;
            _shopEntry.ApplyPositionDeltaSize(_product.OverrideEntryDeltaSize);
            _usingService.Used += UpdateRequested;
        }

        private void OnGetWithReward()
        {
            var _ads = Game.AdverisingService;

            _ads.ShowRewarded(OwnProduct);
        }

        private void OnUse()
        {
            if (_propertyService.IsOwned(_product) == false)
                throw new InvalidOperationException("Trying to use item, when it is not bought");
            
            _usingService.Use(_product);
            _shopEntry.SetEntryState(ShopEntry.EntryState.selected);
        }

        private void OnUseClicked()
        {
            Game.AudioService.PlayShopUseButtonOnClick();
            OnUse();
        }

        private void UpdateRequested(IAcquirable obj)
        {
            if (obj.Guid == _product.Guid)
            {
                _shopEntry.SetEntryState(ShopEntry.EntryState.owned);
            }
        }

        private void UpdateRequested((EquippedType, IShopProduct) product)
        {
            bool sameType = _product.GetType() == product.Item2.GetType();
            if(sameType && _shopEntry.State != ShopEntry.EntryState.closed)
            {
                //_shopEntry.SetEntryState(
                //    _shopEntry.State == ShopEntry.EntryState.selected ?
                //    ShopEntry.EntryState.owned :
                //    ShopEntry.EntryState.selected);
                _shopEntry.SetEntryState(ShopEntry.EntryState.owned);
            }
            //if (sameType && _shopEntry.Use.gameObject.activeInHierarchy)
            //{
            //    _shopEntry.Use.interactable = product.Item2 != _product;
            //}
        }

        private void OnBuyWithYans()
        {
            _iapService.TryBuy(
                _product as IYandexIapProduct, 
                onSuccess: OwnProduct, 
                onError: () => PlayerTriedToBuyAndIapRaisedError?.Invoke());
        }

        private void OwnProduct()
        {
            _propertyService.Own(_product);
            OnUse();
            Game.AudioService.PlayShopBuyButtonOnClick();
        }

   

        private void OnBuyWithCoins()
        {
            if (_moneyService.HasEnoughCoins(_product.CoinsCost) == false)
            {
                PlayerTriedToBuyAndInssufficientFunds?.Invoke();
                return;
            }
            
            _moneyService.Spend(_product.CoinsCost);
            OwnProduct();
        }
    }
}
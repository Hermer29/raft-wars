using System;
using DefaultNamespace.Skins;
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

            _shopEntry.Use.onClick.AddListener(OnUseClicked);
            _shopEntry.BuyForCoins.onClick.AddListener(OnBuyWithCoins);
            _shopEntry.BuyForYans.onClick.AddListener(OnBuyWithYans);

            if (propertyService.IsOwned(_product))
            {
                if (_usingService.IsUsed(_product))
                {
                    _shopEntry.Use.interactable = false;
                    _usingService.Use(_product);
                }
                MarkAsBought();
            }
            else
            {
                MarkAsUnbought();
            }

            _shopEntry.ApplyPositionDeltaSize(_product.OverrideEntryDeltaSize);
            _usingService.Used += UpdateRequested;
        }

        private void OnUse()
        {
            if (_propertyService.IsOwned(_product) == false)
                throw new InvalidOperationException("Trying to use item, when it is not bought");
            
            _usingService.Use(_product);
            _shopEntry.Use.interactable = false;
        }

        private void OnUseClicked()
        {
            Game.AudioService.PlayShopUseButtonOnClick();
            OnUse();
        }

        private void UpdateRequested((EquippedType, IShopProduct) product)
        {
            bool sameType = _product.GetType() == product.Item2.GetType();
            if (product.Item2 != _product && sameType && _shopEntry.Use.gameObject.activeInHierarchy)
            {
                _shopEntry.Use.interactable = true;
            }
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
            MarkAsBought();
            OnUse();
            Game.AudioService.PlayShopBuyButtonOnClick();
        }

        private void MarkAsBought()
        {
            _shopEntry.SetActiveBuyingBlock(false);
            _shopEntry.SetActiveUsingBlock(true);
        }

        private void MarkAsUnbought()
        {
            _shopEntry.SetActiveBuyingBlock(true);
            _shopEntry.SetActiveUsingBlock(false);
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
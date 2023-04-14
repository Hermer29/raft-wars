using System;
using DefaultNamespace.Skins;
using InputSystem;
using Services;
using RaftWars.Infrastructure;

namespace Skins
{
    public class ShopProductPresenter
    {
        private readonly Entry _entry;
        private readonly IShopProduct _product;
        private readonly YandexIAPService _iapService;
        private readonly PlayerMoneyService _moneyService;
        private readonly PlayerUsingService _usingService;
        private readonly PropertyService _propertyService;

        public event Action PlayerTriedToBuyAndInssufficientFunds;
        public event Action PlayerTriedToBuyAndIapRaisedError;
        
        public ShopProductPresenter(Entry entry, IShopProduct product, YandexIAPService iapService, PlayerMoneyService moneyService,
            PlayerUsingService usingService, PropertyService propertyService)
        {
            _entry = entry;
            _product = product;
            _iapService = iapService;
            _moneyService = moneyService;
            _usingService = usingService;
            _propertyService = propertyService;

            _entry.Use.onClick.AddListener(OnUseClicked);
            _entry.BuyForCoins.onClick.AddListener(OnBuyWithCoins);
            _entry.BuyForYans.onClick.AddListener(OnBuyWithYans);

            if (propertyService.IsOwned(_product))
            {
                if (_usingService.IsUsed(_product))
                {
                    _entry.Use.interactable = false;
                    _usingService.Use(_product);
                }
                MarkAsBought();
            }
            else
            {
                MarkAsUnbought();
            }

            _entry.ApplyPositionDeltaSize(_product.OverrideEntryDeltaSize);
            _usingService.Used += UpdateRequested;
        }

        private void OnUse()
        {
            if (_propertyService.IsOwned(_product) == false)
                throw new InvalidOperationException("Trying to use item, when it is not bought");
            
            _usingService.Use(_product);
            _entry.Use.interactable = false;
        }

        private void OnUseClicked()
        {
            Game.AudioService.PlayShopUseButtonOnClick();
            OnUse();
        }

        private void UpdateRequested((EquippedType, IShopProduct) product)
        {
            bool sameType = _product.GetType() == product.Item2.GetType();
            if (product.Item2 != _product && sameType && _entry.Use.gameObject.activeInHierarchy)
            {
                _entry.Use.interactable = true;
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
            _entry.SetActiveBuyingBlock(false);
            _entry.SetActiveUsingBlock(true);
        }

        private void MarkAsUnbought()
        {
            _entry.SetActiveBuyingBlock(true);
            _entry.SetActiveUsingBlock(false);
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
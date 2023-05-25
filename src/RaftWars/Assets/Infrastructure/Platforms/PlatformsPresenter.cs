using System;
using DefaultNamespace;
using DefaultNamespace.Skins;
using InputSystem;
using RaftWars.Infrastructure.Services;
using Services;
using SpecialPlatforms;

namespace Infrastructure.Platforms
{
    public class PlatformsPresenter : IDisposable
    {
        private readonly PlatformEntry _entry;
        private readonly SpecialPlatform _platform;
        private readonly AdvertisingService _advertisingService;
        private readonly PropertyService _propertyService;
        private readonly OwningSequence<SpecialPlatform> _sequence;
        private readonly LevelService _levelService;
        private readonly PlayerMoneyService _moneyService;
        private readonly YandexIAPService _iapService;

        private const int UpgradeLevelLimit = 6;
        private const int UpgradeLevelAdvertisingOnlyLimit = 4;
        private const bool UntilUpgradeLimitUpgradableOnlyByAdvertising = true;

        public PlatformsPresenter(PlatformEntry entry, SpecialPlatform platform, AdvertisingService advertisingService, 
            PropertyService propertyService, OwningSequence<SpecialPlatform> sequence, LevelService levelService, PlayerMoneyService moneyService,
            YandexIAPService iapService)
        {
            _levelService = levelService;
            _moneyService = moneyService;
            _iapService = iapService;
            _propertyService = propertyService;
            _entry = entry;
            _platform = platform;
            _advertisingService = advertisingService;
            _sequence = sequence;
            _propertyService.PropertyOwned += OnAcquiredOtherPlatform;
            
            UpdateView(platform);
        }

        private void UpdateView(SpecialPlatform specialPlatform)
        {
            _entry.WritePositionName(specialPlatform.Illustration, specialPlatform.LocalizedName);

            if (IsOwned(specialPlatform))
            {
                if (TryClaimThatMaxUpgrade(specialPlatform))
                    return;
                
                if (HasLimitOfUpgradesByCoins(specialPlatform))
                    return;
                
                ShowAcquiredAndUpgradable();
                return;
            }

            if (_sequence.IsCanBeOwned(specialPlatform))
            {
                if (EnsureThatRequiredLevelCompleted(specialPlatform))
                {
                    ShowAcquirable();
                    return;
                }
                ShowNotAcquirableBecauseOfLevel();
                return;
            }
            InformThatPreviousPlatformMustBeOwnedBefore();
        }

        private void ShowAcquirable()
        {
            UnsubAllButtons();
            
            _entry.ShowAcquirable();
            _entry.SetCost(2);
            _entry.PurchaseByAdvertising.gameObject.SetActive(true);
            _entry.PurchaseByAdvertising.onClick.AddListener(UnlockByAdvertising);
            _entry.PurchaseForYans.interactable = true;
            _entry.PurchaseForYans.onClick.AddListener(UnlockByYans);
        }

        private bool EnsureThatRequiredLevelCompleted(SpecialPlatform specialPlatform)
        {
            var condition = specialPlatform.RequiredLevel < _levelService.Level;
            if (condition == false)
                return false;
            UnsubAllButtons();
            
            _entry.ShowUpgradableByAdvertising(specialPlatform.UpgradedLevel);
            _entry.PurchaseByAdvertising.onClick.AddListener(UpgradeByAdvertising);
            return true;
        }
        
        private bool IsOwned(SpecialPlatform specialPlatform) 
            => _propertyService.IsOwned(specialPlatform);

        private bool HasLimitOfUpgradesByCoins(SpecialPlatform specialPlatform) 
            => UntilUpgradeLimitUpgradableOnlyByAdvertising && TryMakeUpgradableByAdvertising(specialPlatform);

        private bool TryClaimThatMaxUpgrade(SpecialPlatform specialPlatform)
        {
            bool condition = specialPlatform.UpgradedLevel >= UpgradeLevelLimit;
            if (condition == false)
                return false;
            
            _entry.InformThatMaximumUpgrade(specialPlatform.UpgradedLevel);
            
            _entry.Upgrade.onClick.RemoveAllListeners();
            _entry.PurchaseForYans.onClick.RemoveAllListeners();
            _entry.PurchaseByAdvertising.onClick.RemoveAllListeners();
            
            _entry.PurchaseByAdvertising.interactable = false;
            _entry.PurchaseForYans.interactable = false;
            _entry.Upgrade.interactable = false;
            return true;
        }

        private bool TryMakeUpgradableByAdvertising(SpecialPlatform specialPlatform)
        {
            var condition = specialPlatform.UpgradedLevel >= UpgradeLevelAdvertisingOnlyLimit;
            if (condition == false)
                return false;
            
            UnsubAllButtons();
            _entry.ShowUpgradableByAdvertising(specialPlatform.UpgradedLevel);
            _entry.PurchaseByAdvertising.onClick.AddListener(UpgradeByAdvertising);
            return true;
        }

        private void InformThatPreviousPlatformMustBeOwnedBefore() 
            => _entry.InformThatPreviousPlatformMustBeOwnedBefore();

        private void OnAcquiredOtherPlatform(IAcquirable acquirable) 
            => UpdateView(_platform);

        private void ShowNotAcquirableBecauseOfLevel()
        {
            UnsubAllButtons();
            
            _entry.InformAboutAcquisitionLevelConstraint(_platform.RequiredLevel);
            _entry.SetCost(2);
            _entry.PurchaseByAdvertising.gameObject.SetActive(false);
            _entry.PurchaseForYans.onClick.AddListener(UnlockByYans);
            _entry.PurchaseForYans.gameObject.SetActive(true);
            _entry.PurchaseForYans.interactable = true;
        }

        private void ShowAcquiredAndUpgradable()
        {
            UnsubAllButtons();

            _entry.ShowUpgradableByMoney(
                cost: _platform.UpgradeCost, 
                upgradedLevel: _platform.UpgradedLevel);
            _entry.SetCost(1);
            
            _entry.Upgrade.onClick.AddListener(UpgradeByCoins);
            _entry.PurchaseByAdvertising.onClick.AddListener(UpgradeByAdvertising);
            _entry.PurchaseForYans.onClick.AddListener(UpgradeByYans);
        }

        private void UnlockByYans()
        {
            var product = new SimpleProduct(_platform.ProductIDForAcquirement, 2);
            _iapService.TryBuy(product, onSuccess: Unlock, null);
        }

        private void UnlockByAdvertising() 
            => _advertisingService.ShowRewarded(Unlock);

        private void UpgradeByAdvertising() 
            => _advertisingService.ShowRewarded(Upgrade);

        private void UpgradeByYans()
        {
            var product = new SimpleProduct(_platform.ProductIDForUpgrade, 1);
            _iapService.TryBuy(product, Upgrade, null);
        }

        private void UpgradeByCoins()
        {
            if (_moneyService.TrySpendCoins(_platform.UpgradeCost))
                Upgrade();
        }

        private void Upgrade()
        {
            UnsubAllButtons();
            _platform.IncrementUpgradeLevel();
            UpdateView(_platform);
        }

        private void Unlock()
        {
            UnsubAllButtons();
            _propertyService.Own(_platform);
            UpdateView(_platform);
        }

        private void UnsubAllButtons()
        {
            _entry.Upgrade.onClick.RemoveAllListeners();
            _entry.PurchaseForYans.onClick.RemoveAllListeners();
            _entry.PurchaseByAdvertising.onClick.RemoveAllListeners();
        }

        public void Dispose() 
            => _propertyService.PropertyOwned -= OnAcquiredOtherPlatform;
    }
}
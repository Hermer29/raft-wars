using DefaultNamespace.Skins;
using InputSystem;
using RaftWars.Infrastructure.Services;
using Services;
using SpecialPlatforms;

namespace Infrastructure.Platforms
{
    public class PlatformsPresenter
    {
        private readonly PlatformEntry _entry;
        private readonly SpecialPlatform _platform;
        private readonly AdvertisingService _advertisingService;
        private readonly PropertyService _propertyService;
        private readonly OwningSequence<SpecialPlatform> _sequence;
        private readonly LevelService _levelService;
        private readonly PlayerMoneyService _moneyService;

        private const int UpgradeLevelLimit = 6;
        private const int UpgradeLevelAdvertisingOnlyLimit = 4;
        
        public PlatformsPresenter(PlatformEntry entry, SpecialPlatform platform, AdvertisingService advertisingService, 
            PropertyService propertyService, OwningSequence<SpecialPlatform> sequence, LevelService levelService, PlayerMoneyService moneyService)
        {
            _levelService = levelService;
            _moneyService = moneyService;
            _propertyService = propertyService;
            _entry = entry;
            _platform = platform;
            _advertisingService = advertisingService;
            _sequence = sequence;
            _propertyService.PropertyOwned += OnAcquiredOtherPlatform;
            
            Initialize(platform);
        }

        private void Initialize(SpecialPlatform specialPlatform)
        {
            _entry.WritePositionName(specialPlatform.Illustration, specialPlatform.LocalizedName);

            if (_propertyService.IsOwned(specialPlatform))
            {
                if (specialPlatform.UpgradedLevel >= UpgradeLevelLimit)
                {
                    _entry.InformThatMaximumUpgrade(specialPlatform.UpgradedLevel);
                    return;
                }
                if (specialPlatform.UpgradedLevel >= UpgradeLevelAdvertisingOnlyLimit)
                {
                    _entry.ShowUpgradableByAdvertising(specialPlatform.UpgradedLevel);
                    _entry.UpgradeForAdvertising.onClick.RemoveAllListeners();
                    _entry.UpgradeForAdvertising.onClick.AddListener(UpgradeForAdvertising);
                    return;
                }
                ShowAcquiredAndUpgradable();
                return;
            }

            if (_sequence.IsCanBeOwned(specialPlatform))
            {
                if (EnsureThatRequiredLevelCompleted(specialPlatform))
                {
                    MakeAcquirableByAdvertising();
                    return;
                }
                InformThatLevelMustBeCompleted(specialPlatform.RequiredLevel);
                return;
            }
            InformThatPreviousPlatformMustBeOwnedBefore();
        }

        private void UpgradeForAdvertising()
        {
            _advertisingService.ShowRewarded(ExecuteUpgrade);
        }

        private bool EnsureThatRequiredLevelCompleted(SpecialPlatform specialPlatform)
        {
            return specialPlatform.RequiredLevel < _levelService.Level;
        }

        private void InformThatPreviousPlatformMustBeOwnedBefore()
        {
            _entry.InformThatPreviousPlatformMustBeOwnedBefore();
        }

        private void OnAcquiredOtherPlatform(IAcquirable acquirable)
        {
            Initialize(_platform);
        }

        private void InformThatLevelMustBeCompleted(int requiredLevel)
        {
            _entry.InformThatLevelMustBeCompleted(level: requiredLevel);
        }

        private void MakeAcquirableByAdvertising()
        {
            _entry.ShowAcquirableByAdvertising();
            _entry.OpenForAdvertising.onClick.RemoveAllListeners();
            _entry.OpenForAdvertising.onClick.AddListener(AcquireByAdvertising);
        }

        private void ShowAcquiredAndUpgradable()
        {
            _entry.ShowUpgradableByMoney(
                cost: _platform.UpgradeCost, 
                upgradedLevel: _platform.UpgradedLevel);
            _entry.Upgrade.onClick.RemoveAllListeners();
            _entry.Upgrade.onClick.AddListener(Upgrade);
        }

        private void AcquireByAdvertising()
        {
            _advertisingService.ShowRewarded(() =>
            {
                _entry.OpenForAdvertising.onClick.RemoveAllListeners();
                _platform.IncrementUpgradeLevel();
                ShowAcquiredAndUpgradable();
                _propertyService.Own(_platform);
            });
        }

        private void Upgrade()
        {
            if (_moneyService.TrySpendCoins(_platform.UpgradeCost))
                ExecuteUpgrade();
        }

        private void ExecuteUpgrade()
        {
            _platform.IncrementUpgradeLevel();
            Initialize(_platform);
            return; //В ситуации если требуется оптмизация, что обновления оказались слишком тяжелыми
                    //(врядли это случится)
            
            if (_platform.UpgradedLevel > UpgradeLevelLimit)
            {
                _entry.InformThatMaximumUpgrade(_platform.UpgradedLevel);
                return;
            }
            if (_platform.UpgradedLevel > UpgradeLevelAdvertisingOnlyLimit)
            {
                _entry.SetUpgradeOnlyForAdvertising(_platform.UpgradedLevel);
                return;
            }

            _entry.ShowUpgradableByMoney(_platform.UpgradeCost, _platform.UpgradedLevel);
        }
    }
}
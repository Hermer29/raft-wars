using DefaultNamespace.Skins;
using InputSystem;
using RaftWars.Infrastructure.Services;
using Services;
using SpecialPlatforms;

namespace Infrastructure.Platforms
{
    public class PlatformsPresenter
    {
        private const int UpgradeLevelLimit = 5;
        private readonly PlatformEntry _entry;
        private readonly SpecialPlatform _platform;
        private readonly AdvertisingService _advertisingService;
        private readonly PropertyService _propertyService;
        private readonly OwningSequence<SpecialPlatform> _sequence;
        private readonly LevelService _levelService;
        private readonly PlayerMoneyService _moneyService;

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
            _entry.ShowInformation(specialPlatform.Illustration, specialPlatform.LocalizedName);

            if (_propertyService.IsOwned(specialPlatform))
            {
                if (specialPlatform.UpgradedLevel >= UpgradeLevelLimit)
                {
                    _entry.InformThatMaximumUpgrade(specialPlatform.UpgradedLevel);
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
            if (acquirable.Guid == _platform.Guid)
                return;

            if (_propertyService.IsOwned(_platform))
                return;

            if (acquirable is not SpecialPlatform)
                return;

            if (_sequence.IsCanBeOwned(_platform))
            {
                if (EnsureThatRequiredLevelCompleted(_platform))
                {
                    MakeAcquirableByAdvertising();
                    return;
                }
                InformThatLevelMustBeCompleted(_platform.RequiredLevel);
                return;
            }
            InformThatPreviousPlatformMustBeOwnedBefore();
        }

        private void InformThatLevelMustBeCompleted(int specialPlatformRequiredLevel)
        {
            _entry.InformThatLevelMustBeCompleted(level: specialPlatformRequiredLevel);
        }

        private void MakeAcquirableByAdvertising()
        {
            _entry.ShowAcquirableByAdvertising();
            _entry.OpenForAdvertising.onClick.AddListener(AcquireByAdvertising);
        }

        private void ShowAcquiredAndUpgradable()
        {
            _entry.ShowAcquiredAndUpgradable(
                cost: _platform.UpgradeCost, 
                upgradedLevel: _platform.UpgradedLevel);
            _entry.Upgrade.onClick.AddListener(Upgrade);
        }

        private void AcquireByAdvertising()
        {
            _advertisingService.ShowRewarded(() =>
            {
                _entry.OpenForAdvertising.onClick.RemoveAllListeners();
                ShowAcquiredAndUpgradable();
                _propertyService.Own(_platform);
            });
        }

        private void Upgrade()
        {
            if (_moneyService.TrySpendCoins(_platform.UpgradeCost))
            {
                _platform.IncrementUpgradeLevel();
                if (_platform.UpgradedLevel >= UpgradeLevelLimit)
                {
                    _entry.InformThatMaximumUpgrade(_platform.UpgradedLevel);
                    return;
                }
                _entry.ShowAcquiredAndUpgradable(_platform.UpgradeCost, _platform.UpgradedLevel);
            }
        }
    }
}
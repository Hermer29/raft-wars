using LanguageChanger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.Platforms
{
    public class PlatformEntry : MonoBehaviour
    {
        [SerializeField] private Image _illustration;
        [SerializeField] private TMP_Text _currentUpgradedLevel;
        
        [Header("Acquired")]
        [SerializeField] private TMP_Text _costText;

        [Header("Not acquirable")] 
        [SerializeField] private Image _tintAndLock;
        [SerializeField] private GameObject _lockExplanationRoot;
        [SerializeField] private TMP_Text _lockExplanation;
        [SerializeField] private TMP_Text _platformName;
        [Space(10)] 
        [SerializeField] private GameObject _maximumUpgradeRoot;

        [field: SerializeField] public Button UpgradeForAdvertising { get; private set; }
        [field: SerializeField] public Button Upgrade { get; private set; }
        [field: SerializeField] public Button OpenForAdvertising { get; private set; }

        private LocalizationService _provider;
        
        public void Construct(LocalizationService provider)
        {
            _provider = provider;
        }
        
        public void WritePositionName(Sprite platformIllustration, TextName platformName)
        {
            _illustration.sprite = platformIllustration;
            _platformName.text = _provider[platformName];
            HideMaxNotificationLimit();
        }

        public void InformThatPreviousPlatformMustBeOwnedBefore()
        {
            WriteLockExplanation(TextName.PreviousPlatformMustBeOwnedBefore);
            HideMaxNotificationLimit();
            HideUpgradeForAdvertising();
        }

        public void InformThatLevelMustBeCompleted(int level)
        {
            InformAboutAcquisitionLevelConstraint(level);
            HideMaxNotificationLimit();
            HideUpgradeForAdvertising();
        }

        public void ShowAcquirableByAdvertising()
        {
            MakeAcquirableByAdvertising();
            HideLockExplanation();
            HideUpgrade();
            SetCurrentUpgradeLevelAsZero();
            HideMaxNotificationLimit();
            ShowLocked();
            HideUpgradeForAdvertising();
        }

        public void ShowUpgradableByAdvertising(int level)
        {
            UpgradeForAdvertising.gameObject.SetActive(true);
            HideLockExplanation();
            HideUpgrade();
            HideLockTint();
            HideAcquisitionByAdvertising();
            HideMaxNotificationLimit();
            ShowCurrentUpgradeLevel(level);
        }

        public void ShowUpgradableByMoney(int cost, int upgradedLevel)
        {
            HideAcquisitionByAdvertising();
            ShowUpgradeForAdvertising();
            HideLockExplanation();
            ShowCurrentUpgradeLevel(upgradedLevel);
            ShowUpgradeCost(cost);
            HideMaxNotificationLimit();
            HideUpgradeForAdvertising();
        }

        public void InformThatMaximumUpgrade(int upgradedLevel)
        {
            HideAcquisitionByAdvertising();
            HideUpgrade();
            HideLockExplanation();
            ShowCurrentUpgradeLevel(upgradedLevel);
            ShowMaximumUpgrade();
            UpgradeForAdvertising.gameObject.SetActive(false);
        }

        public void SetUpgradeOnlyForAdvertising(int platformUpgradedLevel)
        {
            _currentUpgradedLevel.text = platformUpgradedLevel.ToString();
            UpgradeForAdvertising.gameObject.SetActive(true);
            OpenForAdvertising.gameObject.SetActive(false);
            Upgrade.gameObject.SetActive(false);
            _tintAndLock.gameObject.SetActive(false);
            _lockExplanationRoot.SetActive(false);
            _maximumUpgradeRoot.SetActive(false);
        }

        private void ShowMaximumUpgrade()
        {
            _maximumUpgradeRoot.SetActive(true);
        }

        private void ShowUpgradeCost(int cost)
        {
            _costText.text = cost.ToString();
        }

        private void ShowCurrentUpgradeLevel(int upgradedLevel)
        {
            _currentUpgradedLevel.text = upgradedLevel.ToString();
        }

        private void SetCurrentUpgradeLevelAsZero()
        {
            _currentUpgradedLevel.text = 0.ToString();
        }

        private void MakeAcquirableByAdvertising()
        {
            OpenForAdvertising.gameObject.SetActive(true);
        }

        private void HideLockTint()
        {
            _tintAndLock.gameObject.SetActive(false);
        }

        private void HideMaxNotificationLimit()
        {
            _maximumUpgradeRoot.SetActive(false);
        }

        private void ShowUpgradeForAdvertising()
        {
            Upgrade.gameObject.SetActive(true);
        }

        private void HideAcquisitionByAdvertising()
        {
            OpenForAdvertising.gameObject.SetActive(false);
        }

        private void HideLockExplanation()
        {
            _lockExplanationRoot.SetActive(false);
            HideLockTint();
        }

        private void HideUpgrade()
        {
            Upgrade.gameObject.SetActive(false);
        }

        private void ShowLocked()
        {
            _tintAndLock.gameObject.SetActive(true);
        }

        private void WriteLockExplanation(TextName explanation)
        {
            _lockExplanationRoot.SetActive(true);
            _lockExplanation.text = _provider[explanation];
        }

        private void InformAboutAcquisitionLevelConstraint(int level)
        {
            _lockExplanationRoot.SetActive(true);
            ParametrizedLocalizableString parametrizedLocaleString = _provider.GetParametrized(TextName.LevelRequired);
            parametrizedLocaleString.EnterParameter(0, level);
            _lockExplanation.text = parametrizedLocaleString.ToString();
        }

        private void HideUpgradeForAdvertising()
        {
            UpgradeForAdvertising.gameObject.SetActive(false);
        }
    }
}
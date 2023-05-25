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
        [SerializeField] private TMP_Text _cost;
        
        [field: SerializeField] public Button PurchaseByAdvertising { get; private set; }
        [field: SerializeField] public Button Upgrade { get; private set; }
        [field: SerializeField] public Button PurchaseForYans { get; private set; }

        private LocalizationService _provider;
        
        public void Construct(LocalizationService provider)
        {
            _provider = provider;
        }
        
        public void WritePositionName(Sprite platformIllustration, TextName platformName)
        {
            _illustration.sprite = platformIllustration;
            _platformName.text = _provider[platformName];
            HideMaxUpgrade();
        }

        public void InformThatPreviousPlatformMustBeOwnedBefore()
        {
            WriteLockExplanation(TextName.PreviousPlatformMustBeOwnedBefore);
            HideMaxUpgrade();
            HideUpgradeForAdvertising();
        }

        public void ShowAcquirable()
        {
            EnableYansButton();
            EnableAdvertisingButton();
            HideLockExplanation();
            HideUpgrade();
            SetCurrentUpgradeLevelAsZero();
            HideMaxUpgrade();
            ShowLocked();
        }

        public void ShowUpgradableByAdvertising(int level)
        {
            EnableAdvertisingButton();
            HideLockExplanation();
            HideUpgrade();
            HideLockTint();
            HideAcquisitionByAdvertising();
            HideMaxUpgrade();
            ShowCurrentUpgradeLevel(level);
        }

        public void ShowUpgradableByMoney(int cost, int upgradedLevel)
        {
            ShowUpgradeForAdvertising();
            HideLockExplanation();
            ShowCurrentUpgradeLevel(upgradedLevel);
            ShowUpgradeCost(cost);
            HideMaxUpgrade();
        }

        public void InformThatMaximumUpgrade(int upgradedLevel)
        {
            HideAcquisitionByAdvertising();
            HideUpgrade();
            HideLockExplanation();
            ShowCurrentUpgradeLevel(upgradedLevel);
            ShowMaximumUpgrade();
            HideAdvertisingButton();
        }

        public void SetCost(int cost)
        {
            _cost.text = cost.ToString();
        }

        private void HideAdvertisingButton()
        {
            PurchaseByAdvertising.gameObject.SetActive(false);
        }

        private void ShowMaximumUpgrade()
        {
            _maximumUpgradeRoot.SetActive(true);
        }

        private void ShowUpgradeCost(int cost)
        {
            _costText.text = cost.ToString();
        }

        private void EnableAdvertisingButton()
        {
            PurchaseByAdvertising.gameObject.SetActive(true);
        }

        private void ShowCurrentUpgradeLevel(int upgradedLevel)
        {
            _currentUpgradedLevel.text = upgradedLevel.ToString();
        }

        private void SetCurrentUpgradeLevelAsZero()
        {
            _currentUpgradedLevel.text = 0.ToString();
        }

        private void EnableYansButton()
        {
            PurchaseForYans.gameObject.SetActive(true);
        }

        private void HideLockTint()
        {
            _tintAndLock.gameObject.SetActive(false);
        }

        private void HideMaxUpgrade()
        {
            _maximumUpgradeRoot.SetActive(false);
        }

        private void ShowUpgradeForAdvertising()
        {
            Upgrade.gameObject.SetActive(true);
        }

        private void HideAcquisitionByAdvertising()
        {
            PurchaseForYans.gameObject.SetActive(false);
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
            PurchaseByAdvertising.gameObject.SetActive(false);
        }
    }
}
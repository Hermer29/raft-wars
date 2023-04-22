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

        [field: SerializeField] public Button Upgrade { get; private set; }
        [field: SerializeField] public Button OpenForAdvertising { get; private set; }

        private DescriptionProvider _provider;
        
        public void Construct(DescriptionProvider provider)
        {
            _provider = provider;
        }
        
        public void ShowInformation(Sprite platformIllustration, TextName platformName)
        {
            _illustration.sprite = platformIllustration;
            _platformName.text = _provider[platformName];
            _maximumUpgradeRoot.SetActive(false);
        }

        public void ShowAcquirableByAdvertising()
        {
            OpenForAdvertising.gameObject.SetActive(true);
            _lockExplanationRoot.SetActive(false);
            Upgrade.gameObject.SetActive(false);
            _currentUpgradedLevel.text = 0.ToString();
            _maximumUpgradeRoot.SetActive(false);
            ShowLocked();
        }

        private void ShowLocked()
        {
            _tintAndLock.gameObject.SetActive(true);
        }

        public void InformThatPreviousPlatformMustBeOwnedBefore()
        {
            _lockExplanationRoot.SetActive(true);
            _lockExplanation.text = _provider[TextName.PreviousPlatformMustBeOwnedBefore];
            _maximumUpgradeRoot.SetActive(false);
        }

        public void InformThatLevelMustBeCompleted(int level)
        {
            _lockExplanationRoot.SetActive(true);
            ParametrizedLocalizableString parametrizedLocaleString = _provider.GetParametrized(TextName.LevelRequired);
            parametrizedLocaleString.EnterParameter(0, level);
            _lockExplanation.text = parametrizedLocaleString.ToString();
            _maximumUpgradeRoot.SetActive(false);
        }

        public void ShowAcquiredAndUpgradable(int cost, int upgradedLevel)
        {
            OpenForAdvertising.gameObject.SetActive(false);
            Upgrade.gameObject.SetActive(true);
            
            _tintAndLock.gameObject.SetActive(false);
            _lockExplanationRoot.SetActive(false);
            
            _currentUpgradedLevel.text = upgradedLevel.ToString();
            _costText.text = cost.ToString();
            _maximumUpgradeRoot.SetActive(false);
        }

        public void InformThatMaximumUpgrade(int upgradedLevel)
        {
            OpenForAdvertising.gameObject.SetActive(false);
            Upgrade.gameObject.SetActive(false);
            _tintAndLock.gameObject.SetActive(false);
            _lockExplanationRoot.SetActive(false);
            _currentUpgradedLevel.text = upgradedLevel.ToString();
            _maximumUpgradeRoot.SetActive(true);
        }
    }
}
using Infrastructure;
using UnityEngine;
using UnityEngine.UI;
using RaftWars.Infrastructure;
using TMPro;
using Visual;

namespace Interface
{
    public class Hud : MonoBehaviour
    {
        [Header("UI")]
        public GameObject tapToPlay;
        public GameObject failedPanel;
        public GameObject blackBG;
        public GameObject stagePanel;
        public Slider progressFill;
        public Slider sliderOnLoseScreen;
        
        [Space]
        public Text damagePercentText, damageCostText, hpPercentText, hpCostText;
        public TMP_Text progressText;
        public Text platformsCountPrev, platformsCountAdd;
        public Text warriorsCountPrev, warriorsCountAdd;
        public Text powerCountPrev, powerCountAdd;
        public Text healthCountPrev, healthCountAdd;
        public Image _healthUpgradeVideoIcon;
        public Image _damageUpgradeVideoIcon;
        public Image _damageUpgradeCoin;
        public Image _healthUpgradeCoin;
        public GameObject[] _diamondsCounter;
        [SerializeField] public Sprite _completedStageSprite;
        [SerializeField] public Image _nextLevelImage;

        public Joystick Joystick;
        public TMP_Text coinsText;
        public Text diamondsText;

        public Button Replay;
        public Button BuyDamage;
        public Button BuyHealth;
        public Button NextStage;
        public Arrow Arrow;
        public Button PauseButton;
        public TMP_Text[] PreviousLevel;
        public TMP_Text[] NextLevel;
        public SoundButton SoundButton;
        public AdvertisingForStatsButton AdvertisingForStatsButton;
        public BossAppearing BossAppearing;
        public SliderWithPercents NewSpecialPlatformProgress;
        public Button MoneyForAdvertisingEndMenu;
        public Button Revive;

        public void ShowBonusWindow()
        {
            AdvertisingForStatsButton.Show();
        }

        private void Start()
        {
            if(Game.FeatureFlags.DiamondsEnabledInGame == false)
            {
                HideDiamondsCounter();
            }
            else
            {
                foreach (GameObject counter in _diamondsCounter)
                {
                    counter.SetActive(true);
                }
            }

            sliderOnLoseScreen.value = 0;
            stagePanel.gameObject.SetActive(false);
            failedPanel.gameObject.SetActive(false);
        }

        public void HideDiamondsCounter()
        {
            foreach (GameObject counter in _diamondsCounter)
            {
                counter.SetActive(false);
            }
        }

        public void ShowCoins(int coins)
        {
            coinsText.text = coins.ToString();
        }

        public void ShowGems(int gems)
        {
            diamondsText.text = gems.ToString();
        }

        public void ShowPreviousLevel(string currentLevel)
        {
            foreach (TMP_Text tmpText in PreviousLevel)
            {
                tmpText.text = currentLevel;
            }
        }

        public void ShowNextLevel(string currentLevel)
        {
            foreach (TMP_Text tmpText in NextLevel)
            {
                tmpText.text = currentLevel;
            }
        }

        public void HideBonusWindow()
        {
            AdvertisingForStatsButton.Hide();
        }
    }
}
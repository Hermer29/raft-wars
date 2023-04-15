using UnityEngine;
using UnityEngine.UI;
using RaftWars.Infrastructure;
using TMPro;

namespace Interface
{
    public class Hud : MonoBehaviour
    {
        [Header("UI")]
        public GameObject tapToPlay;
        public GameObject winPanel;
        public GameObject failedPanel;
        public GameObject blackBG;
        public GameObject stagePanel;
        public Slider progressFill;
        
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

        public Joystick Joystick;
        public TMP_Text coinsText;
        public Text diamondsText;

        public Button Replay;
        public Button Continue;
        public Button BuyDamage;
        public Button BuyHealth;
        public Button NextStage;
        public Arrow Arrow;
        public Button PauseButton;
        public TMP_Text PreviousLevel;
        public TMP_Text NextLevel;
        public Image Tint;
        public SoundButton SoundButton;
        public AdvertisingForStatsButton AdvertisingForStatsButton;
        public BossAppearing BossAppearing;

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
    }
}
﻿using UnityEngine;
using UnityEngine.UI;

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
        public Image progressFill;
        [Space]
        public Text damagePercentText, damageCostText, hpPercentText, hpCostText;
        public Text progressText;
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
        public Text coinsText;
        public Text diamondsText;

        public Button Replay;
        public Button Continue;
        public Button BuyDamage;
        public Button BuyHealth;
        public Button NextStage;

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
using System;
using TMPro;
using TurretMinigame.Player;
using UnityEngine;
using UnityEngine.UI;

namespace TurretMinigame
{
    public class TurretMinigameHud : MonoBehaviour
    {
        [Header("Rewards")] 
        [SerializeField] private GameObject _rewardsRoot;
        [SerializeField] private TMP_Text _rewardsText;
        [SerializeField] private TMP_Text _adsRewardText;

        [Header("Stats")] 
        [SerializeField] private TMP_Text _killCount;
        [SerializeField] private TMP_Text _timePlayed;

        [Header("Windows")] 
        [SerializeField] private GameObject _onStart;
        [SerializeField] private GameObject _advertisingWindow;
        [SerializeField] private Image _previousGunPlacement;

        public Button TakeNormalAmount;
        public Button TakeAdvertisingAmount;
        public Button UpgradeTurretForAdvertising;
        public PlayerEnemiesView PlayerEnemiesView;

        public event Action ClickedOnScreen;

        private void Start()
        {
            _rewardsRoot.SetActive(false);
            HideAdvertisingOffer();
        }

        public void OnStartClicked()
        {
            ClickedOnScreen?.Invoke();
            _onStart.SetActive(false);
        }

        public void ShowMenu(int killCount, int timePlayed, int rewardedAmount, int rewardForAds)
        {
            _rewardsRoot.SetActive(true);
            _killCount.text = killCount.ToString();
            _rewardsText.text = rewardedAmount.ToString();
            _adsRewardText.text = rewardForAds.ToString();
            _timePlayed.text = timePlayed.ToString();
        }

        public void ShowAdvertisingOffer(Sprite prevIllustration)
        {
            _advertisingWindow.SetActive(true);
            _previousGunPlacement.sprite = prevIllustration;
        }

        public void HideAdvertisingOffer()
        {
            _advertisingWindow.SetActive(false);
        }
    }
}
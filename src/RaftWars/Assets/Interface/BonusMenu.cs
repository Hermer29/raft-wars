using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Infrastructure;
using InputSystem;
using RaftWars.Infrastructure;
using RaftWars.Infrastructure.Services;
using RaftWars.Pickables;
using SpecialPlatforms;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class BonusMenu : MonoBehaviour
    {
        private AdvertisingService _advertising;
        private YandexIAPService _iap;
        private IEnumerable<Pickable> _platforms;
        private IPrefsService _prefsService;
        public AdvertisingForStatsButton FirstStart;

        [SerializeField] private Button _showAdvertisingForAdditionalPlatform;
        [SerializeField] private Button _showAdvertisingForAdditionalStats;
        [SerializeField] private Button _buyForYansAdditionalPlatforms;
        [SerializeField] private Button _buyForYansAdditionalStats;
        [SerializeField] private Image _specialPlatformIllustration;

        private AttachablePlatform _selectedPlatform;
        
        private void Awake()
        {
            _advertising = Game.AdverisingService;
            _iap = Game.IAPService;
            _prefsService = CrossLevelServices.PrefsService;
        }

        private void SelectPlatform()
        {
            _selectedPlatform = (AttachablePlatform)_platforms.Random();
            _specialPlatformIllustration.sprite = _selectedPlatform.platform.GetComponent<StatsHolder>()
                .Platform.SpRewardIllustration;
        }

        private void Subscribe()
        {
            _showAdvertisingForAdditionalPlatform.onClick.AddListener(
                () => _advertising.ShowRewarded(GiveAdditionalPlatform));

            _showAdvertisingForAdditionalStats.onClick.AddListener(
                () => _advertising.ShowRewarded(GiveAdditionalStats));

            _buyForYansAdditionalPlatforms.onClick.AddListener(
                () => _iap.TryBuy("extraplatform", 1, GiveAdditionalPlatform));

            _buyForYansAdditionalStats.onClick.AddListener(
                () => _iap.TryBuy("amplificat", 1, GiveAdditionalStats));
        }

        private void GiveAdditionalStats()
        {
            Game.PlayerService.Amplify(10);
            Continuation();
        }

        private void GiveAdditionalPlatform()
        {
            Game.PlayerService.AddPlatform(_selectedPlatform.platform);
            Continuation();
        }

        private void Continuation()
        {
            Game.GameManager.StartGame();
            Hide();
        }

        public void Show()
        {
            _platforms = AllServices.GetSingle<IEnumerable<Pickable>>();
            if (_prefsService.GetInt("FirstStart_BonusMenu", 0) == 1)
            {
                _prefsService.SetInt("FirstStart_BonusMenu", 1);
                gameObject.SetActive(false);
                FirstStart.Show();
                return;
            }
            FirstStart.Hide();
            gameObject.SetActive(true);
            SelectPlatform();
            Subscribe();
        }

        public void Hide()
        {
            if (FirstStart.Shown)
            {
                FirstStart.Hide();
            }
            gameObject.SetActive(false);
        }
    }
}
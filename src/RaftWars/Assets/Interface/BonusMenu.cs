using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Infrastructure;
using InputSystem;
using LanguageChanger;
using RaftWars.Infrastructure;
using RaftWars.Infrastructure.Services;
using RaftWars.Pickables;
using SpecialPlatforms;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class BonusMenu : MonoBehaviour
    {
        private AdvertisingService _advertising;
        private YandexIAPService _iap;
        private IEnumerable<Pickable> _platforms;

        [SerializeField] private Button _showAdvertisingForAdditionalPlatform;
        [SerializeField] private Button _showAdvertisingForAdditionalStats;
        [SerializeField] private Button _buyForYansAdditionalPlatforms;
        [SerializeField] private Button _buyForYansAdditionalStats;
        [SerializeField] private Image _specialPlatformIllustration;
        [SerializeField] private TMP_Text _specialPlatformName;
        
        private AttachablePlatform _selectedPlatform;
        private Action _continuation;
        [field: SerializeField] public Button DeclineBonusesPicking { get; set; }

        private void Awake()
        {
            _advertising = Game.AdverisingService;
            _iap = Game.IAPService;
        }

        private void SelectPlatform()
        {
            _selectedPlatform = (AttachablePlatform)_platforms.Random();
            var stats = _selectedPlatform.platform.GetComponent<StatsHolder>().Platform;
            _specialPlatformName.text = FindObjectOfType<LocalizationService>()[stats.LocalizedName];
            _specialPlatformIllustration.sprite = stats.SpRewardIllustration;
        }

        private void Subscribe()
        {
            _showAdvertisingForAdditionalPlatform.onClick.RemoveAllListeners();
            _showAdvertisingForAdditionalStats.onClick.RemoveAllListeners();
            _buyForYansAdditionalPlatforms.onClick.RemoveAllListeners();
            _buyForYansAdditionalStats.onClick.RemoveAllListeners();
            
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
            _continuation.Invoke();
            Hide();
        }

        public void Show(Action continuation)
        {
            _continuation = continuation;
            _platforms = AllServices.GetSingle<IEnumerable<Pickable>>();
            gameObject.SetActive(true);
            SelectPlatform();
            Subscribe();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
using System;
using DefaultNamespace;
using Infrastructure;
using InputSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Monetization
{
    public class PurchasingAdBlock : MonoBehaviour
    {
        private YandexIAPService _iapService;
        private AdvertisingService _advertisingService;

        [SerializeField] private Button _button;

        private void Start()
        {
            _iapService = Game.IAPService;
            _advertisingService = Game.AdverisingService;

            _button.onClick.AddListener(OnButtonClick);

            if (_advertisingService.IsInterstitialPurchased)
            {
                DisableButton();
            }
        }

        private void DisableButton()
        {
            gameObject.SetActive(false);
        }

        private void OnButtonClick()
        {
            var product = new SimpleProduct("adBlock", 19);
            _iapService.TryBuy(product, () => {
                _advertisingService.IsInterstitialPurchased = true;
                DisableButton();
            });
        }
    }
}
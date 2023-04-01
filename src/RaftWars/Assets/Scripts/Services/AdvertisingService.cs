using System;
using Agava.YandexGames;
using UnityEngine;

namespace InputSystem
{
    public class AdvertisingService
    {
        private YandexGamesEnvironment _gamesEnvironment;
        
        public AdvertisingService()
        {
            _gamesEnvironment = new YandexGamesEnvironment();
        }
        
        private static bool SdkNotWorking => Application.isEditor || YandexGamesSdk.IsInitialized == false;

        public void ShowInterstitial()
        {
            if (SdkNotWorking)
            {
                Debug.Log("Interstitial show");
                return;
            }
            InterstitialAd.Show();        
        }

        public void ShowRewarded(Action onRewarded)
        {
            if (SdkNotWorking)
            {
                Debug.Log("Rewarded shown");
                onRewarded?.Invoke();
                return;
            }
            VideoAd.Show(onRewarded);
        }
    }
}
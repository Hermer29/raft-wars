using System;
using Agava.YandexGames;
using UnityEngine;

namespace InputSystem
{
    public class AdvertisingService
    {
        private readonly AudioService _audioService;
        
        public AdvertisingService(AudioService audioService)
        {
            _audioService = audioService;
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
            VideoAd.Show(OnOpen, OnRewarded, OnRewardedClose, OnRewardedError);
        }

        private void OnRewarded()
        {
            
        }

        private void OnOpen()
        {
            
        }

        private void OnRewardedClose()
        {
            
        }

        private void OnRewardedError(string error)
        {
            
        }
    }
}
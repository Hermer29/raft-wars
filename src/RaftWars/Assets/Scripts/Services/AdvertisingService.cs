using System;
using Agava.YandexGames;
using UnityEngine;

namespace InputSystem
{
    public class AdvertisingService
    {
        private bool _previousAudioState;
        private Action _onRewarded;

        public event Action AdvertisingStarted;
        public event Action AdvertisingEnded;
        
        private static bool SdkNotWorking => Application.isEditor || YandexGamesSdk.IsInitialized == false;

        public void ShowInterstitial()
        {
            if (SdkNotWorking)
            {
                Debug.Log("Interstitial shown");
                return;
            }
            InterstitialAd.Show(
                onOpenCallback: OnOpen,
                onCloseCallback: _ => OnAdvertisingEnded(),
                onErrorCallback: _ => OnAdvertisingEnded(),
                onOfflineCallback: OnAdvertisingEnded);        
        }

        public void ShowRewarded(Action onRewarded)
        {
            if (SdkNotWorking)
            {
                Debug.Log("Rewarded shown");
                onRewarded?.Invoke();
                return;
            }
            _onRewarded = onRewarded;

            VideoAd.Show(
                onOpenCallback: OnOpen, 
                onRewardedCallback: OnRewarded, 
                onCloseCallback: OnAdvertisingEnded, 
                onErrorCallback: _ => OnAdvertisingEnded());
        }

        private void OnAdvertisingEnded() => 
            AdvertisingEnded?.Invoke();

        private void OnRewarded()
        {
            _onRewarded.Invoke();
            OnAdvertisingEnded();
        }

        private void OnOpen() => 
            AdvertisingStarted?.Invoke();
    }
}
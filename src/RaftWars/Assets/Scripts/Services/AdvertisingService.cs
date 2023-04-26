using System;
using Agava.YandexGames;
using UnityEngine;

namespace InputSystem
{
    public class AdvertisingService
    {
        private bool _previousAudioState;
        private Action _onRewarded;

        public event Action RewardedStarted;
        public event Action RewardedEnded;
        
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
            RewardedStarted?.Invoke();
            _onRewarded = onRewarded;

            VideoAd.Show(OnOpen, OnRewarded, OnRewardedClose, OnRewardedError);
        }

        private void OnRewarded()
        {
            _onRewarded.Invoke();
            RewardedEnded?.Invoke();
        }

        private void OnOpen()
        {
            RewardedEnded?.Invoke();
        }

        private void OnRewardedClose()
        {
            RewardedEnded?.Invoke();
        }

        private void OnRewardedError(string error)
        {
            RewardedEnded?.Invoke();
        }
    }
}
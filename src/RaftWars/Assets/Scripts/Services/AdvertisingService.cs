using System;
using Agava.YandexGames;
using UnityEngine;

namespace InputSystem
{
    public class AdvertisingService
    {
        private readonly AudioService _audioService;
        private bool _previousAudioState;
        private Action _onRewarded;
        
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
            _previousAudioState = _audioService.State;
            _audioService.SetState(false);
            _onRewarded = onRewarded;

            VideoAd.Show(OnOpen, OnRewarded, OnRewardedClose, OnRewardedError);
        }

        private void OnRewarded()
        {
            _onRewarded.Invoke();
            _audioService.SetState(_previousAudioState);
        }

        private void OnOpen()
        {
            _audioService.SetState(_previousAudioState);
        }

        private void OnRewardedClose()
        {
            _audioService.SetState(_previousAudioState);
        }

        private void OnRewardedError(string error)
        {
            _audioService.SetState(_previousAudioState);
        }
    }
}
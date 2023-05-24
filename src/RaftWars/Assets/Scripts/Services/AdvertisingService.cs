using System;
using System.Collections;
using Agava.YandexGames;
using RaftWars.Infrastructure;
using RaftWars.Infrastructure.Services;
using UnityEngine;

namespace InputSystem
{
    public class AdvertisingService
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly IPrefsService _prefsService;
        
        private bool _previousAudioState;
        private Action _onRewarded;
        private bool _rewardedEnded;

        public event Action AdvertisingStarted;
        public event Action AdvertisingEnded;
        
        private static bool SdkNotWorking => Application.isEditor || YandexGamesSdk.IsInitialized == false;

        public bool IsInterstitialPurchased
        {
            get => _prefsService.GetInt("IsInterstitialPurchased", 0) == 1;
            set => _prefsService.SetInt("IsInterstitialPurchased", 1);
        }

        public AdvertisingService(ICoroutineRunner coroutineRunner, IPrefsService prefsService)
        {
            _prefsService = prefsService;
            _coroutineRunner = coroutineRunner;
        }
        
        public void ShowInterstitial()
        {
            if (SdkNotWorking)
            {
                Debug.Log("Interstitial shown. But we're not in build");
                return;
            }
            if (IsInterstitialPurchased)
            {
                Debug.Log("Ad block bought");
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
            _coroutineRunner.StartCoroutine(WaitWhileAdIsNotEnded());
            
            VideoAd.Show(
                onOpenCallback: OnOpen, 
                onRewardedCallback: OnRewarded, 
                onCloseCallback: OnAdvertisingEnded, 
                onErrorCallback: _ => OnAdvertisingEnded());
        }

        private IEnumerator WaitWhileAdIsNotEnded()
        {
            yield return new WaitWhile(() => _rewardedEnded == false);
            _onRewarded?.Invoke();
            OnAdvertisingEnded();
            _rewardedEnded = false;
        }

        private void OnAdvertisingEnded() => 
            AdvertisingEnded?.Invoke();

        private void OnRewarded()
        {
            _rewardedEnded = true;
        }

        private void OnOpen() => 
            AdvertisingStarted?.Invoke();
    }
}
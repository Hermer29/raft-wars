using System;
using System.Collections;
using Agava.YandexGames;
using Infrastructure;
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
        private bool _rewarded;

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
                onCloseCallback: _ => OnClosed(),
                onErrorCallback: _ => OnClosed(),
                onOfflineCallback: OnClosed);    
        }

        public void ShowRewarded(Action onRewarded)
        {
            GameManager.instance.GamePaused = true;
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
                onCloseCallback: OnClosed, 
                onErrorCallback: _ => OnClosed());
        }

        private void OnClosed()
        {
            GameManager.instance.GamePaused = false;
            if (_rewarded) _onRewarded?.Invoke();
            _rewarded = false;
            AdvertisingEnded?.Invoke();
        }
        
        private void OnRewarded() => _rewarded = true;

        private void OnOpen()
        {
            GameManager.instance.GamePaused = true;
            AdvertisingStarted?.Invoke();
        }
    }
}
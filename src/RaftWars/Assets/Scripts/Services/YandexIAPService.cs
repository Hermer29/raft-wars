using DefaultNamespace.Skins;
using Agava.YandexGames;
using RaftWars.Infrastructure;
using System;
using Infrastructure;
using UnityEngine;

namespace InputSystem
{
    public class YandexIAPService
    {
        public void TryBuy(IYandexIapProduct product, Action onSuccess, Action onError = null)
        {
            if(Application.isEditor == false && Game.FeatureFlags.EnableYandexIap)
            {
                Billing.PurchaseProduct(product.ProductId, 
                _ => onSuccess?.Invoke(), 
                _ => onError?.Invoke());
                return;
            }

            Debug.Log("Yandex IAP disabled because we're not in build. Everything is free for yans");
            onSuccess?.Invoke();
        }
    }
}
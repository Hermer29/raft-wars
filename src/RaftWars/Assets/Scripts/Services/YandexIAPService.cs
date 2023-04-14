using DefaultNamespace.Skins;
using Agava.YandexGames;
using RaftWars.Infrastructure;
using System;
using UnityEngine;

namespace InputSystem
{
    public class YandexIAPService
    {
        public void TryBuy(IYandexIapProduct product, Action onSuccess, Action onError)
        {
            if(Game.FeatureFlags.EnableYandexIap)
            {
                Billing.PurchaseProduct(product.ProductId, 
                _ => onSuccess(), 
                _ => onError());
                return;
            }

            Debug.Log("Yandex IAP disabled. Everything is free for yans");
            onSuccess.Invoke();
        }
    }
}
using DefaultNamespace;
using DefaultNamespace.Skins;
using InputSystem;
using RaftWars.Infrastructure;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VIPOfferPresenter
{
    private VIPOfferWindow window;
    private readonly YandexIAPService iAPService;
    private readonly PlayerMoneyService money;
    private readonly PropertyService propertyService;
    private readonly AdvertisingService adService;
    public static readonly SimpleProduct vipProduct = new SimpleProduct("VIP", 55);
    public Action PurchaseSucces;
    public bool IsOwned => propertyService.IsOwned(vipProduct);
    public VIPOfferPresenter(YandexIAPService iAPService, PlayerMoneyService money, PropertyService propertyService, AdvertisingService adService)
    {
        this.iAPService = iAPService;
        this.money = money;
        this.propertyService = propertyService;
        this.adService = adService;
    }
    public void Show()
    {
        if(!window)
        {
            window = GameFactory.CreateVIPOfferWindow();
        }
        window.Show();
        Construct();
    }
    private void Construct()
    {
        window.onCLose += Window_onCLose;
        window.onShowOffer += Window_onShowOffer;
    }
    private void Window_onShowOffer()
    {
        iAPService.TryBuy("VIP", 55, OnSuccess);
    }
    private void OnSuccess()
    {
        money.AddCoins(5000);
        propertyService.Own(vipProduct);
        PurchaseSucces?.Invoke();
        window.Close();
        adService.IsInterstitialPurchased = true;
    }
    private void Window_onCLose()
    {
        
    }
}
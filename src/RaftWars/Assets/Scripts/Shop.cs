using System;
using System.Collections.Generic;
using DefaultNamespace.Skins;
using InputSystem;
using RaftWars.Infrastructure;
using RaftWars.Infrastructure.AssetManagement;
using Services;
using Skins;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private Transform _colorsParent;
    [SerializeField] private Transform _raftsParent;
    [SerializeField] private Transform _hatsParent;
    [SerializeField] private CanvasGroup _group;
    
    private UiFactory _factory;
    private List<ShopProductPresenter> _entries;
    private YandexIAPService _iapService;
    private PlayerMoneyService _playerMoneyService;
    private PlayerUsingService _playerUsingService;
    private PropertyService _propertyService;

    public void Construct(UiFactory factory, YandexIAPService iapService, PlayerMoneyService playerMoneyService, PlayerUsingService playerUsingService,
        PropertyService propertyService)
    {
        _factory = factory;
        _iapService = iapService;
        _playerMoneyService = playerMoneyService;
        _playerUsingService = playerUsingService;
        _propertyService = propertyService;
        _entries = new List<ShopProductPresenter>();
        
        ShowShopEntries(AssetLoader.LoadHatSkins(), _hatsParent);
        ShowShopEntries(AssetLoader.LoadPlatformSkins(), _raftsParent);
        ShowShopEntries(AssetLoader.LoadPlayerColors(), _colorsParent);
        HideImmediately();
    }

    public void ShowImmediately()
    {
        _group.alpha = 1;
        _group.interactable = true;
        _group.blocksRaycasts = true;
    }

    public void HideImmediately()
    {
        _group.alpha = 0;
        _group.interactable = false;
        _group.blocksRaycasts = false;
    }

    private void ShowShopEntries(IEnumerable<IShopProduct> products, Transform parent)
    {
        foreach (IShopProduct product in products)
        {
            Entry entry = ShowEntry(product, parent);
            _entries.Add(new ShopProductPresenter(entry, product, _iapService, _playerMoneyService, _playerUsingService, _propertyService));
        }
    }

    private Entry ShowEntry(IShopProduct product, Transform uiParent)
    {
        Entry entry = _factory.CreateEntry();
        entry.Show(product);
        entry.transform.SetParent(uiParent, worldPositionStays: true);
        return entry;
    }
}
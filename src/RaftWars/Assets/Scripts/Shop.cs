using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Skins;
using InputSystem;
using Interface;
using RaftWars.Infrastructure;
using RaftWars.Infrastructure.AssetManagement;
using Services;
using Skins;
using Skins.Hats;
using Skins.Platforms;
using UnityEngine;
using UnityEngine.UI;

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
    private ICoroutineRunner _runner;
    
    private bool _initialized;

    public void Construct(UiFactory factory, YandexIAPService iapService, PlayerMoneyService playerMoneyService, PlayerUsingService playerUsingService,
        PropertyService propertyService, ICoroutineRunner runner)
    {
        _factory = factory;
        _iapService = iapService;
        _playerMoneyService = playerMoneyService;
        _playerUsingService = playerUsingService;
        _propertyService = propertyService;
        _entries = new List<ShopProductPresenter>();
        _runner = runner;
        HideImmediately();
        
        TakeSavedOrDefault(AssetLoader.LoadHatSkins(), AssetLoader.LoadPlatformSkins(), AssetLoader.LoadPlayerColors());
    }

    [field: SerializeField] public ScrollDetector Detector;


    private void CreateEntries()
    {
        var hats = AssetLoader.LoadHatSkins();
        ShowShopEntries(hats, _hatsParent);
        var platformSkins = AssetLoader.LoadPlatformSkins();
        ShowShopEntries(platformSkins, _raftsParent);
        var colors = AssetLoader.LoadPlayerColors();
        ShowShopEntries(colors, _colorsParent);
    }

    private void TakeSavedOrDefault(IEnumerable<HatSkin> hatSkins, IEnumerable<PlatformSkin> platformSkins, IEnumerable<PlayerColors> colors)
    {
        TryUseDefaultOrUseFromSave(platformSkins);
        TryUseDefaultOrUseFromSave(colors);
        TryUseDefaultOrUseFromSave(hatSkins);
    }

    private void TryUseDefaultOrUseFromSave(IEnumerable<IShopProduct> platformSkins)
    {
        if (TryTakeDefaultIfRequired(platformSkins) == false)
        {
            UseSavedAsUsed(platformSkins);
        }
    }

    private void UseSavedAsUsed(IEnumerable<IShopProduct> skins)
    {
        _playerUsingService.Use(skins.First(_playerUsingService.IsSavedUsed));
    }

    private bool TryTakeDefaultIfRequired(IEnumerable<IShopProduct> products)
    {
        if (IsInitialUsingRequired(products))
        {
            TakeOneRequiredByDefault(products);
            return true;
        }

        return false;
    }

    private void TakeOneRequiredByDefault(IEnumerable<IShopProduct> products)
    {
        _playerUsingService.Use(products.First(x => x.OwnedByDefault));
    }

    private bool IsInitialUsingRequired(IEnumerable<IShopProduct> hatSkins)
    {
        return hatSkins.Any(x => _playerUsingService.IsSavedUsed(x)) == false;
    }

    public void ShowImmediately()
    {
        if (_initialized == false)
        {
            CreateEntries();
            _initialized = true;
        }
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
            ShopEntry shopEntry = ShowEntry(product, parent);
            _entries.Add(new ShopProductPresenter(shopEntry, product, _iapService, _playerMoneyService, _playerUsingService, _propertyService));
        }
    }

    private ShopEntry ShowEntry(IShopProduct product, Transform uiParent)
    {
        ShopEntry shopEntry = _factory.CreateEntry();
        shopEntry.Show(product);
        shopEntry.transform.SetParent(uiParent, worldPositionStays: false);
        return shopEntry;
    }
}
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
        CreateEntries();
    }

    [field: SerializeField] public ScrollDetector Detector;


    private void CreateEntries()
    {
        // Извлечение метода
        var hats = AssetLoader.LoadHatSkins();
        if (hats.Any(x => _playerUsingService.IsUsed(x)) == false)
        {
            _playerUsingService.Use(hats.First(x => x.OwnedByDefault));
        }

        ShowShopEntries(hats, _hatsParent);
        var platformSkins = AssetLoader.LoadPlatformSkins();
        if (platformSkins.Any(x => _playerUsingService.IsUsed(x)) == false)
        {
            _playerUsingService.Use(platformSkins.First(x => x.OwnedByDefault));
        }

        ShowShopEntries(platformSkins, _raftsParent);
        var colors = AssetLoader.LoadPlayerColors();
        if (colors.Any(x => _playerUsingService.IsUsed(x)) == false)
        {
            _playerUsingService.Use(colors.First(x => x.OwnedByDefault));
        }

        ShowShopEntries(colors, _colorsParent);
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
        entry.transform.SetParent(uiParent, worldPositionStays: false);
        return entry;
    }
}
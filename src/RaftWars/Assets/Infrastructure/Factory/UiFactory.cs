using DefaultNamespace.Skins;
using Infrastructure.Platforms;
using InputSystem;
using RaftWars.Infrastructure.AssetManagement;
using RaftWars.Infrastructure.Services;
using Services;
using Skins;
using static UnityEngine.Object;

namespace RaftWars.Infrastructure
{
    public class UiFactory
    {
        private readonly UiAssetLoader _loader;
        private readonly PlayerMoneyService _playerMoneyService;
        private readonly YandexIAPService _iapService;
        private readonly PlayerUsingService _playerUsingService;
        private readonly PropertyService _propertyService;
        private readonly ICoroutineRunner _coroutineRunner;

        public UiFactory(UiAssetLoader loader, YandexIAPService iapService, PlayerMoneyService playerMoneyService, 
            PlayerUsingService playerUsingService, PropertyService propertyService, ICoroutineRunner coroutineRunner)
        {
            _loader = loader;
            _playerMoneyService = playerMoneyService;
            _iapService = iapService;
            _playerUsingService = playerUsingService;
            _propertyService = propertyService;
            _coroutineRunner = coroutineRunner;
        }

        public ShopEntry CreateEntry()
        {
            return Instantiate(_loader.LoadEntry());
        }

        public Shop CreateShop()
        {
            var shop = Instantiate(_loader.LoadShop());
            shop.HideImmediately();
            var loadMenu = Instantiate(_loader.LoadMenu());
            shop.Construct(this, _iapService, _playerMoneyService, _playerUsingService, _propertyService, _coroutineRunner);
            loadMenu.Construct(shop, Game.GameManager, Game.Hud, AllServices.GetSingle<PlatformsMenu>());
            return shop;
        }
    }
}
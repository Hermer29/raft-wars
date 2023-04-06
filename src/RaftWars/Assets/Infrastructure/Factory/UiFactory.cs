using DefaultNamespace.Skins;
using InputSystem;
using RaftWars.Infrastructure.AssetManagement;
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

        public UiFactory(UiAssetLoader loader, YandexIAPService iapService, PlayerMoneyService playerMoneyService, 
            PlayerUsingService playerUsingService, PropertyService propertyService)
        {
            _loader = loader;
            _playerMoneyService = playerMoneyService;
            _iapService = iapService;
            _playerUsingService = playerUsingService;
            _propertyService = propertyService;
        }

        public Entry CreateEntry()
        {
            return Instantiate(_loader.LoadEntry());
        }

        public Shop CreateShop()
        {
            var shop = Instantiate(_loader.LoadShop());
            var loadMenu = Instantiate(_loader.LoadMenu());
            shop.Construct(this, _iapService, _playerMoneyService, _playerUsingService, _propertyService);
            loadMenu.Construct(shop, Game.GameManager, Game.Hud);
            return shop;
        }
    }
}
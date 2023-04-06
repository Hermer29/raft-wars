using InputSystem;
using RaftWars.Infrastructure;
using RaftWars.Infrastructure.AssetManagement;
using Services;

namespace Infrastructure.States
{
    public class CreateShopState : IState
    {
        public void Exit()
        {
            
        }

        public void Enter()
        {
            YandexIAPService yandexIapService = Game.IAPService;
            PlayerMoneyService moneyService = Game.MoneyService;
            PlayerUsingService usingService = Game.UsingService;
            PropertyService propertyService = Game.PropertyService;
            
            var uiAssets = new UiAssetLoader();
            var uiFactory = new UiFactory(uiAssets, yandexIapService, moneyService, usingService, propertyService);
            Shop shop = uiFactory.CreateShop();
        }
    }
}
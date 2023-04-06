using DefaultNamespace.Skins;
using Skins;
using Skins.Hats;
using Skins.Platforms;

namespace InputSystem
{
    public class PlayerUsingService
    {
        private readonly PlayerService _playerService;

        public PlayerUsingService(PlayerService playerService)
        {
            _playerService = playerService;
        }
        
        public void Use(IShopProduct shopProduct)
        {
            switch (shopProduct)
            {
                case PlayerColors color:
                    _playerService.PlayerInstance.RepaintWith(color);
                    break;
                case PlatformSkin platformSkin:
                    _playerService.PlayerInstance.ApplyPlatformSkin(platformSkin);
                    break;
                case HatSkin hatSkin:
                    _playerService.PlayerInstance.ApplyHat(hatSkin);
                    break;
            }
        }
    }
}
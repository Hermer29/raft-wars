using System;
using DefaultNamespace.Skins;
using RaftWars.Infrastructure.Services;
using Skins;
using Skins.Hats;
using Skins.Platforms;

namespace InputSystem
{
    public class PlayerUsingService
    {
        private readonly PlayerService _playerService;
        private readonly IPrefsService _prefsService;

        public PlayerUsingService(PlayerService playerService, IPrefsService prefsService)
        {
            _playerService = playerService;
            _prefsService = prefsService;
        }

        public event Action<(EquippedType, IShopProduct)> Used;
        
        public void Use(IShopProduct shopProduct)
        {
            switch (shopProduct)
            {
                case PlayerColors color:
                    _playerService.PlayerInstance.RepaintWith(color);
                    SaveEquip(EquippedType.Color, shopProduct);
                    break;
                case PlatformSkin platformSkin:
                    _playerService.PlayerInstance.ApplyPlatformSkin(platformSkin);
                    SaveEquip(EquippedType.PlatformSkin, shopProduct);
                    break;
                case HatSkin hatSkin:
                    _playerService.PlayerInstance.ApplyHat(hatSkin);
                    SaveEquip(EquippedType.Hat, shopProduct);
                    break;
            }
        }

        public bool IsUsed(IShopProduct shopProduct)
        {
            switch (shopProduct)
            {
                case PlayerColors color:
                    return IsEquipped(EquippedType.Color, shopProduct);
                case PlatformSkin platformSkin:
                    return IsEquipped(EquippedType.PlatformSkin, shopProduct);
                case HatSkin hatSkin:
                    return IsEquipped(EquippedType.Hat, shopProduct);
            }

            throw new Exception("Unreachable");
        }

        private void SaveEquip(EquippedType type, IShopProduct ownable)
        {
            _prefsService.SetString(type.ToString(), ownable.Guid);
            Used?.Invoke((type, ownable));
        }

        private bool IsEquipped(EquippedType type, IShopProduct ownable)
        {
            return _prefsService.GetString(type.ToString()) == ownable.Guid;
        }
    }
    
    public enum EquippedType
    {
        Color,
        PlatformSkin,
        Hat
    }
}
using DefaultNamespace.Skins;
using Interface;
using Skins;
using UnityEngine;

namespace RaftWars.Infrastructure.AssetManagement
{
    public class UiAssetLoader
    {
        public ShopEntry LoadEntry()
        {
            return Resources.Load<ShopEntry>(UiAssetConstants.PathToEntry);
        }

        public Shop LoadShop()
        {
            return Resources.Load<Shop>(UiAssetConstants.PathToShop);
        }

        public MenuUi LoadMenu()
        {
            return Resources.Load<MenuUi>(UiAssetConstants.PathToMenu);
        }
    }
}
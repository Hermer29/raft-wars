using DefaultNamespace.Skins;
using Interface;
using Skins;
using UnityEngine;

namespace RaftWars.Infrastructure.AssetManagement
{
    public class UiAssetLoader
    {
        public Entry LoadEntry()
        {
            return Resources.Load<Entry>(UiAssetConstants.PathToEntry);
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
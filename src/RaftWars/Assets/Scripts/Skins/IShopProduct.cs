using UnityEngine;

namespace DefaultNamespace.Skins
{
    public interface IShopProduct : IAcquirable
    {
        Sprite ShopImage { get; }
        int CoinsCost { get; }
        int YansCost { get; }
        Vector2 OverrideEntryDeltaSize { get; }
    }

    public interface IYandexIapProduct : IShopProduct
    {
        string ProductId { get; }
    }

    public interface IAcquirable
    {
        string Guid { get; }
        
        bool OwnedByDefault { get;}
    }
}
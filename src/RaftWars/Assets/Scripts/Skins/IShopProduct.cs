using UnityEngine;

namespace DefaultNamespace.Skins
{
    public interface IShopProduct : IOwnable
    {
        Sprite ShopImage { get; }
        int CoinsCost { get; }
        int YansCost { get; }
        bool OwnedByDefault { get;}
    }

    public interface IOwnable
    {
        string Guid { get; }
    }
}
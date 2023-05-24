using DefaultNamespace.Skins;
using UnityEngine;

namespace DefaultNamespace
{
    public class SimpleProduct : IYandexIapProduct
    {
        public SimpleProduct(string yandexProductId, int cost)
        {
            YansCost = cost;
            ProductId = yandexProductId;
        }
        
        public string Guid { get; }
        public bool OwnedByDefault { get; }
        public Sprite ShopImage { get; }
        public int CoinsCost { get; }
        public int YansCost { get; }
        public Vector2 OverrideEntryDeltaSize { get; }
        public string ProductId { get; }
    }
}
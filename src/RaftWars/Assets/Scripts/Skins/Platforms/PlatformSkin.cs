using DefaultNamespace.Skins;
using UnityEditor;
using UnityEngine;

namespace Skins.Platforms
{
    public class PlatformSkin : MonoBehaviour, IShopProduct
    {
        [field: SerializeField] public Sprite ShopImage { get; private set; }
        [field: SerializeField] public int CoinsCost { get; private set; }
        [field: SerializeField] public int YansCost { get; private set; }
        [field: SerializeField] public bool HasEdges { get; private set; }
        [field: SerializeField] public bool OwnedByDefault { get; private set; }
        [field: SerializeField] public string Guid { get; private set; }
        [field: SerializeField] public Vector2 OverrideEntryDeltaSize { get; private set; }

        
        private void OnValidate()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(Guid))
            {
                Guid = System.Guid.NewGuid().ToString();
                EditorUtility.SetDirty(this);
            }
#endif
        }
    }
}
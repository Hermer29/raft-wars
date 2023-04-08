using System;
using DefaultNamespace.Skins;
using UnityEditor;
using UnityEngine;

namespace Skins
{
    [CreateAssetMenu(fileName = "PlayerColors", menuName = "Create Player Colors", order = 0)]
    public class PlayerColors : ScriptableObject, IShopProduct
    {
        [field: SerializeField] public Material Color { get; private set; }
        [field: SerializeField] public Sprite ShopImage { get; private set; }
        [field: SerializeField] public int CoinsCost { get; private set; }
        [field: SerializeField] public int YansCost { get; private set; }
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
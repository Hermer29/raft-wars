using System;
using DefaultNamespace.Skins;
using UnityEditor;
using UnityEngine;

namespace Skins.Hats
{
    public class HatSkin : MonoBehaviour, IYandexIapProduct
    {
        [field: SerializeField] public Sprite ShopImage { get; private set; } 
        [field: SerializeField] public int CoinsCost { get; private set; } = 300;
        [field: SerializeField] public int YansCost { get; private set; } = 2;
        [field: SerializeField] public bool OwnedByDefault { get; private set; }
        [field: SerializeField] public string Guid { get; private set; }
        [field: SerializeField] public Vector2 OverrideEntryDeltaSize { get; private set; }
        [field: SerializeField] public string ProductId { get; private set; }

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
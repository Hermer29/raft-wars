using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SpecialPlatforms
{
    [CreateAssetMenu(fileName = "SpecialPlatform", menuName = "🚤Create Special platform", order = 0)]
    public class SpecialPlatform : ScriptableObject, ISaveData
    {
        [field: SerializeField] public AssetReference ReferenceToPrefab { get; private set; }
        [field: SerializeField] public string Guid { get; private set; }

        private int _upgradeLevel;
        private bool _owned;

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

        public string GetData()
        {
            var data = new Dictionary<string, string>()
            {
                {"upgradeLevel", _upgradeLevel.ToString()},
                {"owned", _owned.ToString()}
            };
            return JsonConvert.SerializeObject(data);
        }

        public string Key()
        {
            return Guid;
        }

        public void Populate(string data)
        {
            var result = JsonConvert.DeserializeAnonymousType(data,
                new { upgradeLevel = 0, owned = false });
            _owned = result.owned;
            _upgradeLevel = result.upgradeLevel;
        }
    }
}
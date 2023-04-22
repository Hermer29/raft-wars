using System.Collections.Generic;
using Infrastructure.Platforms;
using LanguageChanger;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SpecialPlatforms
{
    [CreateAssetMenu(fileName = "SpecialPlatform", menuName = "🚤Create Special platform", order = 0)]
    public class SpecialPlatform : ScriptableObject, ISavableData, ISequentiallyOwning, ICompletedLevelConditional
    {
        [field: SerializeField] public int Serial { get; private set; }
        [field: SerializeField] public AssetReference ReadyPlatform { get; private set; }
        [field: SerializeField] public AssetReference PickablePlatform { get; private set; }
        [field: SerializeField] public string Guid { get; private set; }
        [field: SerializeField] public bool OwnedByDefault { get; private set; }
        [field: SerializeField] public Sprite Illustration { get; private set; }
        [field: SerializeField] public int RequiredLevel { get; private set; }
        [field: SerializeField] public TextName LocalizedName { get; private set; }

        /// <summary>
        /// [0, Infinity]
        /// </summary>
        public int UpgradedLevel { get; private set; } = 0;
        public int UpgradeCost => CostPerLevel * (UpgradedLevel + 1);
        
        private const int CostPerLevel = 50;

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

        public void IncrementUpgradeLevel()
        {
            UpgradedLevel++;
        }

        string ISavableData.GetData()
        {
            var data = new Dictionary<string, string>()
            {
                {"upgradeLevel", UpgradedLevel.ToString()}
            };
            return JsonConvert.SerializeObject(data);
        }

        string ISavableData.Key()
        {
            return Guid;
        }

        void ISavableData.Populate(string data)
        {
            var result = JsonConvert.DeserializeAnonymousType(data,
                new { upgradeLevel = 0 });
            UpgradedLevel = result.upgradeLevel;
        }
    }
}
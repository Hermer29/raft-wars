using System.Collections.Generic;
using Infrastructure.Platforms;
using LanguageChanger;
using Newtonsoft.Json;
using Units;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SpecialPlatforms
{
    [CreateAssetMenu(fileName = "SpecialPlatform", menuName = "🚤Create Special platform", order = 0)]
    public abstract class SpecialPlatform : ScriptableObject, ISavableData, ISequentiallyOwning, ICompletedLevelConditional, IGenericStatsInformer, 
        IPurchasableSpecialPlatform
    {
        [field: SerializeField] public int Serial { get; private set; }
        [field: SerializeField] public AssetReference ReadyPlatform { get; private set; }
        [field: SerializeField] public AssetReference PickablePlatform { get; private set; }
        [field: SerializeField] public string Guid { get; private set; }
        [field: SerializeField] public bool OwnedByDefault { get; private set; }
        [field: SerializeField] public Sprite Illustration { get; private set; }
        [field: SerializeField] public int RequiredLevel { get; private set; }
        [field: SerializeField] public TextName LocalizedName { get; private set; }
        [field: SerializeField] public Sprite StatIcon { get; private set; }
        [field: SerializeField] public Sprite SpRewardIllustration { get; private set; }

        /// <summary>
        /// [0, Infinity]
        /// </summary>
        public int UpgradedLevel { get; private set; } = 1;
        public int UpgradeCost => CostPerLevel * (UpgradedLevel);
        public abstract ValueType Type { get; }
        public abstract float DefaultAmount { get; }
        private const int CostPerLevel = 50;
        public abstract string ProductIDForUpgrade { get; protected set; }
        public abstract string ProductIDForAcquirement { get; protected set; }

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

        public void IncrementUpgradeLevel() => UpgradedLevel++;

        string ISavableData.GetData()
        {
            var data = new Dictionary<string, string>()
            {
                {"upgradeLevel", UpgradedLevel.ToString()}
            };
            return JsonConvert.SerializeObject(data);
        }

        string ISavableData.Key() => Guid;

        void ISavableData.Populate(string data)
        {
            var result = JsonConvert.DeserializeAnonymousType(data,
                new { upgradeLevel = 0 });
            UpgradedLevel = result.upgradeLevel;
        }
    }
}
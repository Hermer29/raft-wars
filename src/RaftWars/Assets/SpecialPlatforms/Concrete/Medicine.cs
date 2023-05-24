using UnityEngine;

namespace SpecialPlatforms.Concrete
{
    [CreateAssetMenu(menuName = "Special platforms/💊Create medicine")]
    public class Medicine : SpecialPlatform, IHealthIncreasing
    {
        [field: Header("Platform specific")]
        [field: SerializeField] public float DefaultHealthGain { get; private set; }
        [SerializeField] private float HealthBonusUpgradeStep;
        [field: SerializeField] public ValueType ValueType { get; private set; }
        public float HealthValue => HealthBonusUpgradeStep * UpgradedLevel + DefaultHealthGain;
        public override ValueType Type => ValueType;
        public override float DefaultAmount => DefaultHealthGain;
        [field: SerializeField] public override string ProductIDForUpgrade { get; protected set; }
        [field: SerializeField] public override string ProductIDForAcquirement { get; protected set; }

    }
}
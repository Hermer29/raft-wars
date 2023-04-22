using UnityEngine;

namespace SpecialPlatforms.Concrete
{
    [CreateAssetMenu(menuName = "Special platforms/🗼Create tower")]
    public class Tower : SpecialPlatform, IHealthIncreasing
    {
        [field: Header("Platform specific")] 
        [field: SerializeField] public float DefaultHealthGain { get; private set; }
        [SerializeField] private float HealthBonusUpgradeStep;
        [field: SerializeField] public ValueType ValueType { get; private set; }
        public float HealthValue => HealthBonusUpgradeStep * UpgradedLevel + DefaultHealthGain;
    }
}
using UnityEngine;

namespace SpecialPlatforms.Concrete
{
    [CreateAssetMenu(menuName = "Special platforms/💊Create medicine")]
    public class Medicine : SpecialPlatform, IHealthIncreasing
    {
        [field: Header("Platform specific")]
        [SerializeField] private float DefaultHealthGain;
        [SerializeField] private float HealthBonusUpgradeStep;
        [field: SerializeField] public ValueType ValueType { get; private set; }
        public float HealthValue => HealthBonusUpgradeStep * UpgradedLevel + DefaultHealthGain;
    }
}
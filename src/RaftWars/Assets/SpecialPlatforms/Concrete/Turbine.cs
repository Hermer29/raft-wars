using UnityEngine;

namespace SpecialPlatforms.Concrete
{
    [CreateAssetMenu(menuName = "Special platforms/🛬Create turbine")]
    public class Turbine : SpecialPlatform, ISpeedIncreasing
    {
        [Header("Platform specific")]
        [SerializeField] private float _defaultSpeedBonus;
        [SerializeField] private float _speedUpgradeStep;

        [field: SerializeField] public ValueType ValueType { get; private set; }
        public float SpeedBonus => _defaultSpeedBonus + UpgradedLevel * _speedUpgradeStep;
    }
}
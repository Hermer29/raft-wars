using UnityEngine;

namespace SpecialPlatforms.Concrete
{
    [CreateAssetMenu(menuName = "Special platforms/💨Create propeller")]
    public class Propeller : SpecialPlatform, ISpeedIncreasing
    {
        [field: Header("Platform specific")] 
        [field: SerializeField] public float DefaultSpeedBonus { get; private set; }
        [SerializeField] private float _speedUpgradeStep;

        [field: SerializeField] public ValueType ValueType { get; private set; }
        public float SpeedBonus => DefaultSpeedBonus + UpgradedLevel * _speedUpgradeStep;
        public override ValueType Type => ValueType;
        public override float DefaultAmount => DefaultSpeedBonus;
        [field: SerializeField] public override string ProductIDForUpgrade { get; protected set; }
        [field: SerializeField] public override string ProductIDForAcquirement { get; protected set; }

    }
}
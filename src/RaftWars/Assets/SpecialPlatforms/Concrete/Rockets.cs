using UnityEngine;

namespace SpecialPlatforms.Concrete
{
    [CreateAssetMenu(menuName = "Special platforms/🚀Create rockets")]
    public class Rockets : SpecialPlatform, IDamageAmplifyer
    {
        [field: Header("Platform specific")] 
        [field: SerializeField] public float BaseDamage { get; private set; }
        [SerializeField] private float DamageBonusUpgradeStep;
        [field: SerializeField] public ValueType ValueType { get; private set; }
        public float DamageValue => DamageBonusUpgradeStep * UpgradedLevel + BaseDamage;

        public override ValueType Type => ValueType;
        public override float DefaultAmount => BaseDamage;
        [field: SerializeField] public override string ProductIDForUpgrade { get; protected set; }
        [field: SerializeField] public override string ProductIDForAcquirement { get; protected set; }

    }
}
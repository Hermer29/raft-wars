using UnityEngine;

namespace SpecialPlatforms.Concrete
{
    [CreateAssetMenu(menuName = "Special platforms/🚀Create rockets")]
    public class Rockets : SpecialPlatform, IDamageAmplifying
    {
        [field: Header("Platform specific")] 
        [SerializeField] private float DefaultDamageGain;

        [SerializeField] private float DamageBonusUpgradeStep;
        [field: SerializeField] public ValueType ValueType { get; private set; }
        public float DamageValue => DamageBonusUpgradeStep * UpgradedLevel + DefaultDamageGain;
    }
}
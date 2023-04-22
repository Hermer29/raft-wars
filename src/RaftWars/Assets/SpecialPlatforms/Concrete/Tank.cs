using UnityEngine;

namespace SpecialPlatforms.Concrete
{
    [CreateAssetMenu(menuName = "Special platforms/🚎Create tank")]
    public class Tank : SpecialPlatform, IDamageAmplifying
    {
        [field: Header("Platform specific")] 
        [field: SerializeField] public float BaseDamage { get; private set; }
        [SerializeField] private float DamageBonusUpgradeStep;
        [field: SerializeField] public ValueType ValueType { get; private set; }
        public float DamageValue => DamageBonusUpgradeStep * UpgradedLevel + BaseDamage;
    }
}
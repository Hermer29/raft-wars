using UnityEngine;

namespace SpecialPlatforms.Concrete
{
    [CreateAssetMenu(menuName = "Special platforms/⚔Create turret")]
    public class Turret : SpecialPlatform, IDamageAmplifying
    {
        [field: Header("Platform specific")] 
        [field: SerializeField] public float BaseDamage { get; private set; }

        [SerializeField] private float DamageBonusUpgradeStep;
        [field: SerializeField] public ValueType ValueType { get; private set; }
        public float DamageValue => DamageBonusUpgradeStep * UpgradedLevel + BaseDamage;
    }
}
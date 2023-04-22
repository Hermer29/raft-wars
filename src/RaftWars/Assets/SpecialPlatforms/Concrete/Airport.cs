using UnityEngine;

namespace SpecialPlatforms.Concrete
{
    [CreateAssetMenu(menuName = "Special platforms/✈Create airport")]
    public class Airport : SpecialPlatform, IDamageAmplifying
    {
        [field: Header("Platform specific")] 

        [SerializeField] private float DamageBonusUpgradeStep;
        [field: SerializeField] public ValueType ValueType { get; private set; }
        [field: SerializeField] public float BaseDamage { get; private set; }
        public float DamageValue => DamageBonusUpgradeStep * UpgradedLevel + BaseDamage;
    }
}
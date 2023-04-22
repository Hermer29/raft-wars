using UnityEngine;

namespace SpecialPlatforms.Concrete
{
    [CreateAssetMenu(menuName = "Special platforms/🧲Create magnet")]
    public class Magnet : SpecialPlatform
    {
        [Header("Platform specific")] 
        [SerializeField] private float _defaultCollectingRadius;
        [SerializeField] private float _collectingRadiusUpgradeStep;

        public float CollectingRadius => _defaultCollectingRadius + _collectingRadiusUpgradeStep * UpgradedLevel;
    }
}
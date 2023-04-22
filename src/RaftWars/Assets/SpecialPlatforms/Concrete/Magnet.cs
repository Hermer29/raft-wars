using UnityEngine;
using UnityEngine.Serialization;

namespace SpecialPlatforms.Concrete
{
    [CreateAssetMenu(menuName = "Special platforms/🧲Create magnet")]
    public class Magnet : SpecialPlatform
    {
        [field: FormerlySerializedAs("_defaultCollectingRadius")]
        [field: Header("Platform specific")] 
        [field: SerializeField] public float DefaultCollectingRadius { get; private set; }
        [SerializeField] private float _collectingRadiusUpgradeStep;

        public float CollectingRadius => DefaultCollectingRadius + _collectingRadiusUpgradeStep * UpgradedLevel;
    }
}
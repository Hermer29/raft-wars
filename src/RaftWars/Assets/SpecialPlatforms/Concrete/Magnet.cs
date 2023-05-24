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
        public override ValueType Type => ValueType.Relative;
        public override float DefaultAmount => DefaultCollectingRadius;
        [field: SerializeField] public override string ProductIDForUpgrade { get; protected set; }
        [field: SerializeField] public override string ProductIDForAcquirement { get; protected set; }

    }
}
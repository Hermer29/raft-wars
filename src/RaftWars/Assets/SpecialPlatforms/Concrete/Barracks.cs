using UnityEngine;

namespace SpecialPlatforms.Concrete
{
    [CreateAssetMenu(menuName = "Special platforms/🏹Create barracks")]
    public class Barracks : SpecialPlatform
    {
        [field: Header("Platform specific")] 
        [field: SerializeField] public int SpawnPeopleDefaultLimit { get; private set; }
        [field: SerializeField] private int _upgradeLimitStep;
        [field: SerializeField] public int SpawnPeopleTime { get; private set; }
        
        public int SpawnPeopleLimit => SpawnPeopleDefaultLimit + _upgradeLimitStep * UpgradedLevel;
        public override ValueType Type => ValueType.NotSuitable;
        public override float DefaultAmount => SpawnPeopleDefaultLimit;
        [field: SerializeField] public override string ProductIDForUpgrade { get; protected set; }
        [field: SerializeField] public override string ProductIDForAcquirement { get; protected set; }

    }
}
using UnityEngine;

namespace SpecialPlatforms.Concrete
{
    [CreateAssetMenu(menuName = "Special platforms/🏹Create barracks")]
    public class Barracks : SpecialPlatform
    {
        [field: Header("Platform specific")] 
        [field: SerializeField] private int _spawnPeopleDefaultLimit;
        [field: SerializeField] private int _upgradeLimitStep;
        [field: SerializeField] public int SpawnPeopleTime { get; private set; }
        
        public int SpawnPeopleLimit => _spawnPeopleDefaultLimit + _upgradeLimitStep * UpgradedLevel;
    }
}
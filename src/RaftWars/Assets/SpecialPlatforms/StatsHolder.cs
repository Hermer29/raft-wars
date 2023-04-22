using UnityEngine;

namespace SpecialPlatforms
{
    public class StatsHolder : MonoBehaviour
    {
        [field: SerializeField] public SpecialPlatform Platform { get; private set; }
    }
}
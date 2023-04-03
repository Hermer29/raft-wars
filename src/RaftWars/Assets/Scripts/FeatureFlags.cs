using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "FeatureFlags", menuName = "Utility/🛠 Add Feature flags", order = 0)]
    public class FeatureFlags : ScriptableObject
    {
        [field: SerializeField] public bool IMGUIEnabled { get; private set; }
    }
}
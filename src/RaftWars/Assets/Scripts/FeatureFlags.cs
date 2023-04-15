using Unity.VisualScripting;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "FeatureFlags", menuName = "Utility/🛠 Add Feature flags", order = 0)]
    public class FeatureFlags : ScriptableObject
    {
        [field: SerializeField] public bool IMGUIEnabled { get; private set; }
        [field: SerializeField] public bool DiamondsEnabledInGame { get; private set; }
        [field: SerializeField] public bool MenuButtonsFading { get; private set; }
        [field: SerializeField] public bool TutorialEveryTime { get; private set; }
        [field: SerializeField] public bool InitializeYandexGames { get; private set; }
        [field: SerializeField] public PrefsOptions PrefsImplementation {get; private set;}
        [field: SerializeField] public bool EnableYandexIap { get; private set; }
        [field: SerializeField] public float PlayerSpeedIncreasingPerPlatform {get; private set;}
    }
}
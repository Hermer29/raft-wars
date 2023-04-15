using UnityEngine;

namespace Services
{
    [CreateAssetMenu]
    public class FightConstants : UnityEngine.ScriptableObject
    {
        [SerializeField] public float DifferenceWeight = 5f;
        public float DamageWeight = 5f;
        public float FightSpeedModifierDecreasing = 50f;
    }
}
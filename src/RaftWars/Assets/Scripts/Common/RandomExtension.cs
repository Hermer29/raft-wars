using UnityEngine;

namespace DefaultNamespace.Common
{
    public static class RandomExtension
    {
        public static bool ProbabilityCheck(float normalized)
        { 
            return Random.Range(0, 100) <= 100 * normalized;
        }
    }
}
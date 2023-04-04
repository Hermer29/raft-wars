using UnityEngine;

namespace Common
{
    public class GeoMaths
    {
        public static bool AlmostEquals(Vector3 a, Vector3 b)
        {
            return (a - b).sqrMagnitude < 2;
        }
    }
}
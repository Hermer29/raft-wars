using UnityEngine;

namespace Common
{
    public static class VectorExtensions
    {
        public static Vector3 SetY(this Vector3 vector, float value)
        {
            vector.y = value;
            return vector;
        }
    }
}
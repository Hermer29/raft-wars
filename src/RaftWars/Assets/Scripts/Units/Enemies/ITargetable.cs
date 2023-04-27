using UnityEngine;

namespace Units.Enemies
{
    public interface ITargetable
    {
        Vector3 GetRandomTarget();
    }
}
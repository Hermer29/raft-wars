using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public interface IPlatformsCarrier
    {
        IEnumerable<GameObject> GetPlatforms();
    }
}
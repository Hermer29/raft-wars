using System;
using UnityEngine;

namespace Units.Attachables
{
    public class PlatformsVisual : MonoBehaviour
    {
        [SerializeField] private MeshRenderer[] _renderers;

        public void Colorize(Material material)
        {
            foreach (MeshRenderer meshRenderer in _renderers)
            {
                // у баннера второй материал это балка, у остальных это единственный материал
                meshRenderer.material = material;
            }
        }
    }
}
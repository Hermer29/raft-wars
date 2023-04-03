using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InputSystem
{
    public class MaterialsService
    {
        private Material[] _edgesMaterial;

        public MaterialsService()
        {
            _edgesMaterial = Resources.LoadAll<Material>("MaterialOptions");
        }

        public Material GetPlayerMaterial()
        {
            return Resources.Load<Material>("PlayerMaterial");
        }
        
        public Material GetRandom()
        {
            if(_edgesMaterial.Length == 0)
                throw new InvalidOperationException();
            return _edgesMaterial[Random.Range(0, _edgesMaterial.Length)];
        }
    }
}
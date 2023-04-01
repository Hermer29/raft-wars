using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InputSystem
{
    public class MaterialsService
    {
        private Material[] _materials;

        public MaterialsService()
        {
            _materials = Resources.LoadAll<Material>("Options");
        }
        
        public Material GetRandom()
        {
            if(_materials.Length == 0)
                throw new InvalidOperationException();
            return _materials[Random.Range(0, _materials.Length)];
        }
    }
}
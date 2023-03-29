using System;
using UnityEngine;

namespace InputSystem
{
    public class CollectiblesService
    {
        private int _collectiblesAmount;

        public event Action NoCollectiblesLeft;
        
        public void Create()
        {
            _collectiblesAmount++;
            Debug.Log($"Collectible created (count: {_collectiblesAmount}");
        }

        public void Spend()
        {
            _collectiblesAmount--;
            Debug.Log($"Collectible spent (count: {_collectiblesAmount}");
            if(_collectiblesAmount == 0)
                NoCollectiblesLeft?.Invoke();
        }
    }
}
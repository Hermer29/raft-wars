using InputSystem;
using RaftWars.Infrastructure;
using UnityEngine;

namespace RaftWars.Pickables
{
    public abstract class Pickable : MonoBehaviour
    {
        private AttachablePlatform _attachable;
        private CollectiblesService _collectibles;
        
        public bool canTake = true;
        
        private void Start()
        {
            _collectibles = Game.CollectiblesService;
            _collectibles.Create();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<ICanTakePlatform>() == null)
                return;
                
            if(other.gameObject.transform.parent.TryGetComponent<Enemy>(out _))
                return;

            if (canTake == false)
                return;

            _collectibles ??= Game.CollectiblesService;
            _collectibles?.Spend();
            
            TriggerEntered(other);
        }

        protected virtual void TriggerEntered(Collider other)
        {
            
        }
    }
}
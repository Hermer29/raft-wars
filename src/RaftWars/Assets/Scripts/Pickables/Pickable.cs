using Infrastructure;
using InputSystem;
using RaftWars.Infrastructure;
using UnityEngine;

namespace RaftWars.Pickables
{
    public abstract class Pickable : MonoBehaviour, IDraggableByMagnet
    {
        private AttachablePlatform _attachable;
        private CollectiblesService _collectibles;
        
        public bool canTake = true;
        public bool notExcludable;
        
        private void Start()
        {
            _collectibles = Game.CollectiblesService;
            _collectibles.Create();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<ICanTakePlatform>(out var platformTaker) == false)
                return;

            if (canTake == false)
                return;

            if (platformTaker is Platform { isEnemy: false })
            {
                Game.MapGenerator.PickedUp();
            }
            
            _collectibles ??= Game.CollectiblesService;
            _collectibles?.Spend();
            
            TriggerEntered(other);
        }

        protected virtual void TriggerEntered(Collider other)
        {
            
        }
    }
}
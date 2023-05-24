using System;
using System.Linq;
using UnityEngine;

namespace RaftWars.Pickables
{
    [RequireComponent(typeof(BoxCollider))]
    public class PlayerTriggerListener : MonoBehaviour
    {
        public event Action TriggeredPlayerPlatform;
        public event Action TriggerExitPlayer;

        private BoxCollider _collider;
        private readonly Collider[] _overlapResults = new Collider[5];
        private bool _triggered;
        
        private void Start() => _collider = GetComponent<BoxCollider>();

        private void Update()
        {
            ProcessPlayerDetection();
        }

        private void ProcessPlayerDetection()
        {
            if (OverlapsPlayer())
            {
                ProcessTriggerStart();
                return;
            }
            ProcessTriggerEnd();
        }

        private void ProcessTriggerEnd()
        {
            if (!_triggered) return;
            TriggerExitPlayer?.Invoke();
            _triggered = false;
        }

        private void ProcessTriggerStart()
        {
            if (_triggered == false)
            {
                _triggered = true;
                TriggeredPlayerPlatform?.Invoke();
            }
        }

        private bool OverlapsPlayer()
        {
            int overlapAmount = Physics.OverlapBoxNonAlloc(transform.position + _collider.center, _collider.size,
                _overlapResults);
            if (overlapAmount == 0)
                return false;
            return _overlapResults.Any(
                x =>
                    x != null &&
                    x.TryGetComponent(out Platform platform) && 
                    platform.isEnemy == false);
        }
    }
}
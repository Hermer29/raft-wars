using Monetization;
using Unity.VisualScripting;
using UnityEngine;

namespace RaftWars.Pickables
{
    public class PickableNearbyPresenceTrigger : MonoBehaviour
    {
        private PickingRaftPieceAdvertising _raftPieceAdvertising;

        public void Construct(PickingRaftPieceAdvertising raft)
        {
            _raftPieceAdvertising = raft;

            _raftPieceAdvertising.Hide();
            Initialize();
        }

        public void Initialize()
        {
            var referenceCollider = GetComponent<BoxCollider>();
            GameObject obj = CreatePresenceTriggerObject();
            BoxCollider collider = CreatePresenceTriggerCollider(obj, referenceCollider);
            var triggerListener = collider.AddComponent<PlayerTriggerListener>();
            triggerListener.TriggeredPlayerPlatform += OnPlayerTriggerEnter;
            triggerListener.TriggerExitPlayer += OnPlayerTriggerExit;
        }

        private void OnPlayerTriggerEnter() => _raftPieceAdvertising.Show();

        private void OnPlayerTriggerExit() => _raftPieceAdvertising.Hide();

        private static BoxCollider CreatePresenceTriggerCollider(GameObject obj, BoxCollider referenceCollider)
        {
            var presenceTrigger = obj.AddComponent<BoxCollider>();
            presenceTrigger.center = referenceCollider.center;
            var scale = referenceCollider.transform.localScale;
            scale.Scale(referenceCollider.size);
            presenceTrigger.size = scale;
            presenceTrigger.isTrigger = true;
            return presenceTrigger;
        }

        private GameObject CreatePresenceTriggerObject()
        {
            var obj = new GameObject();
            obj.transform.SetParent(transform);
            obj.name = "DynamicGenerated";
            obj.transform.localPosition = Vector3.zero;
            return obj;
        }
    }
}
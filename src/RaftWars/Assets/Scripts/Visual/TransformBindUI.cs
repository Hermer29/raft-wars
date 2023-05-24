using UnityEngine;

namespace Visual
{
    public class TransformBindUI : MonoBehaviour
    {
        public Transform Target;
        
        private const float HudZTargetOffset = 5;
        
        private void Update()
        {
            if (Target == null)
                return;
            Vector3 worldTarget = Target.position;
            worldTarget.z += HudZTargetOffset;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldTarget);
            transform.position = screenPoint;
        }
    }
}
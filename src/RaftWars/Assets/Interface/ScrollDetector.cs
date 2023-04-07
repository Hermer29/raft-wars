using System;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class ScrollDetector : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scroll;

        private float _prevValue;

        public event Action ScrolledDown;
        public event Action ScrolledUp;
        
        private void Start()
        {
            _prevValue = _scroll.verticalNormalizedPosition;
            _scroll.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(Vector2 arg0)
        {
            const float tolerance = 0.5f;
            var delta = arg0.y - _prevValue;

            if(Mathf.Abs(delta) < tolerance)
            {
                if (delta > 0)
                {
                    ScrolledUp?.Invoke();
                }
                else if (delta < 0)
                {   
                    ScrolledDown?.Invoke();
                }
                _prevValue = arg0.y;
            }
           
        }
    }
}
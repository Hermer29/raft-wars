using System;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class ScrollDetector : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scroll;

        public event Action ScrolledDown;
        public event Action ScrolledUp;
        
        private void Start()
        {
            _scroll.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(Vector2 arg0)
        {
            if (arg0.y < 0)
            {
                ScrolledDown?.Invoke();
            }
            else if (arg0.y > 0)
            {
                ScrolledUp?.Invoke();
            }
        }
    }
}
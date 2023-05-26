using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Interface
{
    public class ClickDetector : MonoBehaviour, IPointerClickHandler
    {
        public event Action Clicked;
        
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            Clicked?.Invoke();
        }
    }
}
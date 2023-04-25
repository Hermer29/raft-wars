using UnityEngine;
using UnityEngine.EventSystems;

namespace Interface
{
    public class ClickDetector : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private GameManager _gameManager;
        
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            _gameManager.StartGame();
            Destroy(gameObject);
        }
    }
}
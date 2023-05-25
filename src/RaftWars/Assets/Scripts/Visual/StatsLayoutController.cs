using UnityEngine;
using UnityEngine.UI;

namespace Visual
{
    [ExecuteAlways]
    public class StatsLayoutController : MonoBehaviour
    {
        [SerializeField] private LayoutElement _stats;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RectTransform _transformToMove;
        [SerializeField] private float _initialXPosition;
        [SerializeField] private float _offsetXPosition = -500;
        
        private void Update()
        {
            if (_rectTransform.rect.height < _stats.preferredHeight)
            {
                _transformToMove.anchoredPosition = new Vector2(_initialXPosition + _offsetXPosition, _transformToMove.anchoredPosition.y);
            }
            else
            {
                _transformToMove.anchoredPosition = new Vector2(_initialXPosition, _transformToMove.anchoredPosition.y);
            }
        }
    }
}
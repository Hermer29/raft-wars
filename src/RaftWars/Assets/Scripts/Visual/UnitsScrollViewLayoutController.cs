using System;
using UnityEngine;
using UnityEngine.UI;

namespace Visual
{
    [ExecuteAlways]
    public class UnitsScrollViewLayoutController : MonoBehaviour
    {
        [SerializeField] private VerticalLayoutGroup _layoutGroup;
        [SerializeField] private int _paddingTop;
        [SerializeField] private float _borderingAspectRatio;
        
        private void Update()
        {
            var aspectRatio = Screen.width / Screen.height;
            Debug.Log(aspectRatio);
            if (aspectRatio < _borderingAspectRatio)
            {
                _layoutGroup.padding.top = _paddingTop;
                return;
            }
            _layoutGroup.padding.top = 0;
        }
    }
}
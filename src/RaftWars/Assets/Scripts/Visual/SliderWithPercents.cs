using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Visual
{
    public class SliderWithPercents : MonoBehaviour
    {
        private float _targetValue;
        private float _currentValue;
        
        [SerializeField] private Slider _slider;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _speedChange = 0.05f;

        private void SetPercent(float percent)
        {
            _slider.value = percent;
            _text.text = $"{(int)Mathf.Ceil(percent * 100)}%";
        }

        public void SetValue(float percent)
        {
            _targetValue = percent;
        }

        private void Update()
        {
            _currentValue = Mathf.MoveTowards(_currentValue, _targetValue, 
                _speedChange * Time.deltaTime);
            SetPercent(_currentValue);
        }
    }
}
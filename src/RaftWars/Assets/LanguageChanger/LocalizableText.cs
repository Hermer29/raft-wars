using TMPro;
using UnityEngine;

namespace LanguageChanger
{
    public class LocalizableText : MonoBehaviour
    {
        [SerializeField] protected TextName _name;
        [SerializeField] protected TMP_Text _text;

        private bool _isInitialized;
        private string _actualString;
        private LocalizationService _provider;

        private void OnValidate()
        {
            if(_text == null)
                _text = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;
            var provider = FindObjectOfType<LocalizationService>();
            if (provider == null)
            {
                Debug.LogWarning($"[{nameof(LocalizableText)}] Not found language description provider. Component disabled");
                return;
            }

            _provider = provider;
            
            if(provider.Font != null)
                _text.font = provider.Font;
            _actualString = provider[_name];
            SetText();
        }

        protected virtual void SetText()
        {
            _text.text = _actualString;
        }

        public void SetParameter(string substring, string resolve)
        {
            Initialize();
            _actualString = _actualString.Replace(substring, resolve);
            SetText();
        }

        public string Resolve(TextName platformName)
        {
            Initialize();
            return _provider[platformName];
        }
    }
}
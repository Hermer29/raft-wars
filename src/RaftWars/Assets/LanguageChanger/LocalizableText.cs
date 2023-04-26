using TMPro;
using UnityEngine;

namespace LanguageChanger
{
    public class LocalizableText : MonoBehaviour
    {
        [SerializeField] protected TextName _name;
        [SerializeField] protected TMP_Text _text;
        
        private void Start()
        {
            var provider = FindObjectOfType<LocalizationService>();
            if (provider == null)
            {
                Debug.LogWarning($"[{nameof(LocalizableText)}] Not found language description provider. Component disabled");
                return;
            }
            
            if(provider.Font != null)
                _text.font = provider.Font;
            SetText(provider);
        }

        protected virtual void SetText(LocalizationService provider)
        {
            _text.text = provider[_name];
        }
    }
}
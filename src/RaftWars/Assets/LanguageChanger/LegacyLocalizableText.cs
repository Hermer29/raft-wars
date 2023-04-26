using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LanguageChanger
{
    public class LegacyLocalizableText : MonoBehaviour
    {
        [SerializeField] protected TextName _name;
        [SerializeField] protected Text _text;
        
        private void Start()
        {
            var provider = FindObjectOfType<LocalizationService>();
            if (provider == null)
            {
                Debug.LogWarning($"[{nameof(LocalizableText)}] Not found language description provider. Component disabled");
                return;
            }
        
            SetText(provider);
        }

        protected virtual void SetText(LocalizationService provider)
        {
            _text.text = provider[_name];
        }
    }
}
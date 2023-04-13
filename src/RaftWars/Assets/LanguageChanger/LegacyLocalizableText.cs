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
            var provider = FindObjectOfType<DescriptionProvider>();
            if (provider == null)
            {
                Debug.LogWarning($"[{nameof(LocalizableText)}] Not found language description provider. Component disabled");
                return;
            }
        
            SetText(provider);
        }

        protected virtual void SetText(DescriptionProvider provider)
        {
            _text.text = provider[_name];
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace LanguageChanger
{
    [CreateAssetMenu(fileName = "LanguageDescription", menuName = "Language")]
    public class LanguageDescription : ScriptableObject, IEnumerable<(TextName, string)>
    {
        [SerializeField] private string _languageName;
        [SerializeField] private TMP_FontAsset _font;
        [SerializeField] private LanguageDescriptionPart[] _description;

        public TMP_FontAsset Font => _font;
        
        public IEnumerator<(TextName, string)> GetEnumerator()
        {
            foreach (var descriptionPart in _description)
            {
                yield return (descriptionPart.textName, descriptionPart.textValue);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
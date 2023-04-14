using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace LanguageChanger
{
    public class DescriptionProvider : MonoBehaviour
    {
        public static DescriptionProvider Instance;
        [SerializeField] private LanguageDescription _russian;
        [SerializeField] private LanguageDescription _english;
        [Header("Testing")] [SerializeField] private bool _enableTesting;
        [SerializeField] private string _languageOverride = "ru";

        public TMP_FontAsset Font => GetRelevantDescription().Font;

        private void Start()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public string this[TextName textName]
        {
            get { return GetRelevantDescription().First(x => x.Item1 == textName).Item2; }
        }

        private LanguageDescription GetRelevantDescription()
        {
            if (_enableTesting)
            {
                return _languageOverride != "ru" ? _english : _russian;
            }

            var lang = Agava.YandexGames.YandexGamesSdk.Environment.i18n.lang;
            return lang != "ru" ? _english : _russian;
        }
    }
}
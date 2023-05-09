using System.Linq;
using TMPro;
using UnityEngine;

namespace LanguageChanger
{
    public class LocalizationService : MonoBehaviour
    {
        public static LocalizationService Instance;
        [SerializeField] private LanguageDescription _russian;
        [SerializeField] private LanguageDescription _english;
        [Header("Testing")] [SerializeField] private bool _enableTesting;
        [NaughtyAttributes.ShowIf("_enableTesting")]
        [SerializeField] private string _languageOverride = "ru";

        public TMP_FontAsset Font => GetRelevantDescription().Font;

        private void Start()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public string this[TextName textName]
        {
            get
            {
                (TextName, string) targetRecord = GetRelevantDescription()
                    .FirstOrDefault(x => x.Item1 == textName);
                if (targetRecord == default)
                    return $"{{{textName.ToString().ToUpper()}}}";
                return targetRecord.Item2;
            }
        }

        private LanguageDescription GetRelevantDescription()
        {
            if (_enableTesting)
            {
                return _languageOverride != "ru" ? _english : _russian;
            }
            
            string lang = Application.isEditor ? "ru" : Agava.YandexGames.YandexGamesSdk.Environment.i18n.lang;
            return lang != "ru" ? _english : _russian;
        }

        public ParametrizedLocalizableString GetParametrized(TextName textName)
        {
            LanguageDescriptionParametrizedPart searched = GetRelevantDescription().Parametrized
                .FirstOrDefault(x => x.Name == textName);
            if (searched == null)
                return new ParametrizedLocalizableString($"{{{textName}}}");
            return new ParametrizedLocalizableString(searched.Value);
        }
    }
}
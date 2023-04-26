using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpecialPlatforms.SPRewardState
{
    public class SPRewardWindow : MonoBehaviour
    {
        [SerializeField] private TMP_Text _statText;
        [SerializeField] private Image _statIcon;

        public Button Claim;
        public Button NotClaim;
        
        public void ShowSpecialPlatform(Sprite sprite, SpecialPlatform platform)
        {
            _statIcon.sprite = platform.StatIcon;
            var statsInformer = platform as IGenericStatsInformer;

            switch (statsInformer.Type)
            {
                case ValueType.NotSuitable:
                    HideStatsInfo();
                    break;
                case ValueType.Absolute:
                    ShowIcon(sprite);
                    _statText.text = $"{statsInformer.DefaultAmount}";
                    break;
                case ValueType.Relative:
                    ShowIcon(sprite);
                    _statText.text = $"{Mathf.Floor(statsInformer.DefaultAmount * 100)}%";
                    break;
            }
        }

        private void ShowIcon(Sprite sprite)
        {
            _statIcon.sprite = sprite;
        }

        private void HideStatsInfo()
        {
            _statIcon.gameObject.SetActive(false);
            _statText.gameObject.SetActive(false);
        }
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SpecialPlatforms.SPRewardState
{
    public class SPRewardWindow : MonoBehaviour
    {
        [SerializeField] private TMP_Text _statText;
        [SerializeField] private Image _statIcon;
        [SerializeField] private Animation _illustrationAnimation;
        [SerializeField] private Image _illustration;

        public Button Claim;
        public Button NotClaim;
        
        public void ShowSpecialPlatform(SpecialPlatform platform)
        {
            ShowIcon(platform.StatIcon);
            _illustration.sprite = platform.SpRewardIllustration;
            var statsInformer = platform as IGenericStatsInformer;

            switch (statsInformer.Type)
            {
                case ValueType.NotSuitable:
                    HideStatsInfo();
                    break;
                case ValueType.Absolute:
                    _statText.text = $"{statsInformer.DefaultAmount}";
                    break;
                case ValueType.Relative:
                    _statText.text = $"{Mathf.Floor(statsInformer.DefaultAmount * 100)}%";
                    break;
            }
            _illustrationAnimation.Play();
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
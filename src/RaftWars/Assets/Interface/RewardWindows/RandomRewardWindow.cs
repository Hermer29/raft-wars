using DefaultNamespace.Skins;
using UnityEngine;
using UnityEngine.UI;

namespace Interface.RewardWindows
{
    public class RandomRewardWindow : MonoBehaviour
    {
        [SerializeField] private Image _positionImage;
        [SerializeField] private Animation _animation;
        
        public Button Decline;
        public Button Accept;
        
        public void Show(IShopProduct shownOne)
        {
            _animation.Play();
            _positionImage.sprite = shownOne.ShopImage;
        }
    }
}
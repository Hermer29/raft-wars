using Infrastructure;
using InputSystem;
using LanguageChanger;
using UnityEngine;
using UnityEngine.UI;

namespace Monetization
{
    public class PickingRaftPieceAdvertising : MonoBehaviour
    {
        private AdvertisingService _advertising;
        private AttachablePlatform _pickable;
        
        [SerializeField] private Button _button;
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private LocalizableText _platformName;
        
        public void Construct(AttachablePlatform pickable, TextName platformName)
        {
            _pickable = pickable;
            _platformName.SetParameter("{0}", _platformName.Resolve(platformName));
        }
        
        private void Start()
        {
            _advertising = Game.AdverisingService;
            
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick() 
            => _advertising.ShowRewarded(OnPicked);

        private void OnPicked()
        {
            _pickable.Take(Game.PlayerService.PlayerInstance.GetAnotherPlatform());
            Destroy(gameObject);
        }

        public void Hide() => _group.alpha = 0;

        public void Show() => _group.alpha = 1;
    }
}
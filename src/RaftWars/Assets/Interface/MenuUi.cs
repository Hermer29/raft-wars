using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class MenuUi : MonoBehaviour
    {
        [SerializeField] private Button _onClickGoFight;
        [SerializeField] private Button _onClickGoShop;
        [SerializeField] private CanvasGroup _canvas;
        
        
        private Shop _shop;
        private Hud _hud;
        private bool _inFightState = true;
        private Coroutine _waitingForHideButtons;

        public void Construct(Shop shop, GameManager gameManager, Hud hud)
        {
            _shop = shop;
            _hud = hud;
            
            _onClickGoFight.onClick.AddListener(ShopClosed);
            _onClickGoShop.onClick.AddListener(ShopOpened);
            gameManager.GameStarted += () => gameObject.SetActive(false);
            
            shop.Detector.ScrolledDown += OnScrollDown;
            shop.Detector.ScrolledUp += OnScrollUp;
        }

        private void ShopOpened()
        {
            _inFightState = false;
            _shop.ShowImmediately();
            _hud.tapToPlay.SetActive(false);
        }

        private void ShopClosed()
        {
            _inFightState = true;
            _shop.HideImmediately();
            _hud.tapToPlay.SetActive(true);

            if (_waitingForHideButtons != null)
            {
                StopCoroutine(_waitingForHideButtons);
            }
            Show();
        }

        private IEnumerator HideTimeout()
        {
            yield return new WaitForSeconds(2f);
            Hide();
        }

        private void OnScrollUp()
        {
            ActionStarted();
            Show();
        }

        private void Show()
        {
            _canvas.alpha = 1;
            _canvas.interactable = true;
            _canvas.blocksRaycasts = true;
        }

        private void OnScrollDown()
        {
            ActionStarted();
            Hide();
        }

        private void Hide()
        {
            if(_inFightState)
                return;
            _canvas.alpha = 0;
            _canvas.interactable = false;
            _canvas.blocksRaycasts = false;
        }

        private void ActionStarted()
        {
            if (_waitingForHideButtons != null)
            {
                StopCoroutine(_waitingForHideButtons);
            }
            _waitingForHideButtons = StartCoroutine(HideTimeout());
        }
    }
}
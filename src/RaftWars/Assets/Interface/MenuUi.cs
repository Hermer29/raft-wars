using System;
using System.Collections;
using System.Linq;
using Infrastructure;
using Infrastructure.Platforms;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class MenuUi : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvas;
        [SerializeField] private MenuItem[] _items;

        [Serializable]
        public class MenuItem
        {
            public WindowType WindowType;
            public Image SelectionBackground;
            public Button Open;

            public void Select()
            {
                SelectionBackground.enabled = true;
            }

            public void Deselect()
            {
                SelectionBackground.enabled = false;
            }
        }

        [Serializable]
        public enum WindowType
        {
            Shop, Fight, Platforms
        }
        
        private Shop _shop;
        private Hud _hud;
        private Coroutine _waitingForHideButtons;
        private PlatformsMenu _platformsMenu;

        public void Construct(Shop shop, GameManager gameManager, Hud hud, PlatformsMenu platformsMenu)
        {
            _shop = shop;
            _hud = hud;
            _platformsMenu = platformsMenu;
            
            if(Game.FeatureFlags.MenuButtonsFading)
            {
                shop.Detector.ScrolledDown += OnScrollDown;
                shop.Detector.ScrolledUp += OnScrollUp;
            }

            foreach (MenuItem item in _items)
            {
                item.Open.onClick.AddListener(() => OnSelected(item));
            }
            
            gameManager.GameStarted += () => gameObject.SetActive(false);
            OnSelected(_items.First(x => x.WindowType == WindowType.Fight));
        }

        private void OnSelected(MenuItem item)
        {
            SelectNewDeselectOthers(item);
            switch (item.WindowType)
            {
                case WindowType.Fight:
                    _shop.HideImmediately();
                    _hud.tapToPlay.SetActive(true);
                    _platformsMenu.HideImmediately();
                    break;
                case WindowType.Platforms:
                    _platformsMenu.ShowImmediately();
                    _shop.HideImmediately();
                    _hud.tapToPlay.SetActive(false);
                    break;
                case WindowType.Shop:
                    _shop.ShowImmediately();
                    _platformsMenu.HideImmediately();
                    _hud.tapToPlay.SetActive(false);
                    break;
            }
        }

        private void SelectNewDeselectOthers(MenuItem item)
        {
            item.Select();
            foreach (var otherItem in _items.Except(new[] { item }))
            {
                otherItem.Deselect();
            }
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
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    public class MenuUi : MonoBehaviour
    {
        [SerializeField] private Button _onClickGoFight;
        [SerializeField] private Button _onClickGoShop;
        
        private Shop _shop;
        private Hud _hud;

        public void Construct(Shop shop, GameManager gameManager, Hud hud)
        {
            _shop = shop;
            _hud = hud;
            
            _onClickGoFight.onClick.AddListener(ShopClosed);
            _onClickGoShop.onClick.AddListener(ShopOpened);
            gameManager.GameStarted += () => gameObject.SetActive(false);
        }

        private void ShopOpened()
        {
            _shop.ShowImmediately();
            _hud.tapToPlay.SetActive(false);
        }

        private void ShopClosed()
        {
            _shop.HideImmediately();
            _hud.tapToPlay.SetActive(true);
        }
    }
}
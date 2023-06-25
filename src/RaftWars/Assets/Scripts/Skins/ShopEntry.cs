using DefaultNamespace.Skins;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Skins
{
    public class ShopEntry : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _yansCost;
        [SerializeField] private TMP_Text _coinsCost;
        [SerializeField] private GameObject _buyingBlock;
        [SerializeField] private GameObject _selectedBlock;
        //[SerializeField] private GameObject _usingBlock;

        public Button BuyForYans;
        public Button BuyForCoins;
        public Button Use;
        public Button Reward;
        
        private EntryState state;
        public EntryState State => state;

        public void Show(IShopProduct product)
        {
            _image.sprite = product.ShopImage;
            _yansCost.text = product.YansCost.ToString();
            _coinsCost.text = product.CoinsCost.ToString();
        }

        //public void SetActiveBuyingBlock(bool state)
        //{
        //    _buyingBlock.SetActive(state);
        //}

        //public void SetActiveUsingBlock(bool state)
        //{
        //    _usingBlock.SetActive(state);
        //}
        public void SetEntryState(EntryState state)
        {
            this.state = state;
            bool isBayed = state != EntryState.closed;
            bool isSelected = state == EntryState.selected;

            _buyingBlock.SetActive(!isBayed);
            Reward.gameObject.SetActive(!isBayed);

            _selectedBlock.SetActive(isSelected);
        }
        public void ApplyPositionDeltaSize(Vector2 deltaSize)
        {
            var rect = _image.rectTransform.rect;
            _image.rectTransform.sizeDelta += deltaSize;
        }

        public enum EntryState
        {
            selected,
            closed,
            owned
        }
    }
}
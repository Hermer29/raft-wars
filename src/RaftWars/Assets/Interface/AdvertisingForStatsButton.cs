using InputSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    [RequireComponent(typeof(Button))]
    public class AdvertisingForStatsButton : MonoBehaviour
    {
        [SerializeField] private GameObject _rootWindow;
        
        private AdvertisingService _advertisingService;
        private PlayerService _playerService;
        private GameManager _gameManager;

        private bool _active;
        
        private const int Amount = 10;
        public bool Shown => _active;

        public void Construct(AdvertisingService ads, PlayerService playerService, GameManager gameManager)
        {
            _playerService = playerService;
            _advertisingService = ads;
            GetComponent<Button>().onClick.AddListener(ShowAds);
            gameManager.GameStarted += GameStarted;
            _gameManager = gameManager;
        }

        private void GameStarted()
        {
            _rootWindow.SetActive(false);
            _active = false;
        }

        private void ShowAds()
        {
            _advertisingService.ShowRewarded(OnRewarded);
        }

        private void OnRewarded()
        {
            _playerService.PlayerInstance.IncreaseStats(Amount);
            _gameManager.StartGame();
        }

        public void Show()
        {
            _active = true;
            _rootWindow.SetActive(true);
        }

        public void Hide()
        {
            _active = false;
            _rootWindow.SetActive(false);
        }
    }
}
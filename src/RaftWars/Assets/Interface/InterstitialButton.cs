using Infrastructure;
using InputSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    [RequireComponent(typeof(Button))]
    public class InterstitialButton : MonoBehaviour
    {
        private AdvertisingService _advertising;

        private void Start()
        {
            _advertising = Game.AdverisingService;
            GetComponent<Button>().onClick.AddListener(() => _advertising.ShowInterstitial());
        }
    }
}
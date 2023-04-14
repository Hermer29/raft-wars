using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    [RequireComponent(typeof(Button), typeof(Image))]
    public class SoundButton : MonoBehaviour
    {
        private AudioService _audioService;
        private Image _image;

        public void Construct(AudioService audioService)
        {
            GetComponent<Button>().onClick.AddListener(OnChangeState);
            _image = GetComponent<Image>();
            _audioService = audioService;
        }

        private void OnChangeState()
        {
            _audioService.SetState(!_audioService.State);
            Color color = _image.color;
            color.a = _audioService.State ? 1 : .7f;
            _image.color = color;
        }
    }
}
using RaftWars.Infrastructure;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField] private Button _continue;
    [SerializeField] private Canvas _menu;
    private Button _openingPause;

    public void Construct(Button openingPause)
    {
        _openingPause = openingPause;
        openingPause.onClick.AddListener(Show);
        openingPause.gameObject.SetActive(false);
        _continue.onClick.RemoveAllListeners();
        _menu.gameObject.SetActive(false);
    }

    public void ShowButton()
    {
        _openingPause.gameObject.SetActive(true);
    }

    private void Show()
    {
        Game.Hud.Arrow.Disable();
        Game.InputService.Disable();
        Game.GameManager.GamePaused = true;
        _menu.gameObject.SetActive(true);
        _continue.onClick.AddListener(Hide);
    }

    private void Hide()
    {
        Game.Hud.Arrow.Enable();
        Game.InputService.Enable();
        Game.GameManager.GamePaused = false;
        _continue.onClick.RemoveAllListeners();
        _menu.gameObject.SetActive(false);
    }
}
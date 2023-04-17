using RaftWars.Infrastructure;
using UnityEngine;
using UnityEngine.UI;
using Services;

public class Pause : MonoBehaviour
{
    [SerializeField] private Button _continue;
    [SerializeField] private Canvas _menu;
    private Button _openingPause;
    private FightService _fight;

    public void Construct(Button openingPause, FightService fight)
    {
        _fight = fight;
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
        _fight.TryPause();
        Game.Hud.Arrow.Disable();
        Game.InputService.Disable();
        Game.GameManager.GamePaused = true;
        _menu.gameObject.SetActive(true);
        _continue.onClick.AddListener(Hide);
    }

    private void Hide()
    {
        _fight.TryUnpause();
        Game.Hud.Arrow.Enable();
        Game.InputService.Enable();
        Game.GameManager.GamePaused = false;
        _continue.onClick.RemoveAllListeners();
        _menu.gameObject.SetActive(false);
    }
}
using System.Collections;
using RaftWars.Infrastructure.Services;
using UnityEngine;

public class AudioService : MonoBehaviour
{
    private IPrefsService _prefsService;
    private bool _state;

    [SerializeField] private AudioSource _fight;
    [SerializeField] private AudioSource _coinPickedUp;
    [SerializeField] private AudioSource _platformPickingUp;
    [SerializeField] private AudioSource _loose;
    [SerializeField] private AudioSource _swimmingSound;
    [SerializeField] private AudioSource _standingSound;
    [SerializeField] private AudioSource _peoplePickedUp;
    [SerializeField] private AudioSource _shopUse;
    [SerializeField] private AudioSource _shopBuy;
    [SerializeField] private AudioSource _bossIsHere;
    [SerializeField] private AudioSource _battleCry;

    private const string AudioKey = "Audio";

    public void Construct(IPrefsService prefsService)
    {
        _prefsService = prefsService;
        _state = prefsService.GetInt(AudioKey, 1) == 1;
        SetState(_state);
        _standingSound.Play();
    }

    public bool State => _state;

    public void SetState(bool enabled)
    {
        AudioListener.pause = !enabled;
        AudioListener.volume = enabled ? 1 : 0;
        _prefsService.SetInt(AudioKey, enabled ? 1 : 0);
        _state = enabled;
    }

    public void PlayFightAudio()
    {
        _fight.Play();
        _battleCry.Play();
    }
    
    public void StopFightAudio()
    {
        _battleCry.Stop();
        _fight.Stop();
    }

    public void PlayLooseSound()
    {
        _loose.Play();
    }

    public void PlaySwimmingSound()
    {
        _swimmingSound.Play();
        _standingSound.Pause();
    }

    public void StopPlayingSwimmingSound()
    {
        _swimmingSound.Pause();
        _standingSound.Play();
    }

    public void PlayPlatformPickingUpSound()
    {
        _platformPickingUp.Stop();
        _platformPickingUp.Play();
    }

    public void CoinPickedUp()
    {
        _coinPickedUp.Stop();
        _coinPickedUp.Play();
    }

    public void PeoplePickedUp()
    {
        _peoplePickedUp.Stop();
        _peoplePickedUp.Play();
    }

    public void PlayShopUseButtonOnClick()
    {
        _shopUse.Play();
    }

    public void PlayShopBuyButtonOnClick()
    {
        _shopBuy.Play();
    }

    public void PlayBossIsHere()
    {
        _bossIsHere.Play();
    }
}
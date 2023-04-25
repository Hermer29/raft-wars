using System;
using System.Collections;
using DG.Tweening;
using RaftWars.Infrastructure;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossAppearing : MonoBehaviour
{
    private const float TimeToGenerateBoss = 20f;
    private float _timeout;
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _bossIsHere;

    private MapGenerator _generator;
    private GameManager _gameManager;
    private Action _generateBoss;

    private void Awake()
    {
        _root.SetActive(false);
        _bossIsHere.gameObject.SetActive(false);
    }

    public void QueryBossSpawn(Action generateBoss)
    {
        _root.SetActive(true);
        _generateBoss = generateBoss;
        _timeout = Game.FeatureFlags.TimeoutBeforeBossSpawn;
        StartSpawnTimeout();
    }

    private void StartSpawnTimeout()
    {
        StartCoroutine(QueryBossSpawnProcess());
    }

    private IEnumerator QueryBossSpawnProcess()
    {
        yield return null;
        _root.GetComponent<CanvasGroup>().alpha = 1;
        _slider.value = 1;
        _slider.DOValue(0, _timeout)
            .OnComplete(OnTimeoutEnded);
    }

    private void OnDisable()
    {
        _slider.DOKill();
    }

    private void OnTimeoutEnded()
    {
        _generateBoss?.Invoke();
        _generateBoss = null;
        _root.GetComponent<CanvasGroup>().alpha = 0;
        _bossIsHere.gameObject.SetActive(true);
        ShowAppearingAnimation();
        Game.AudioService.PlayBossIsHere();

        void OnWritingShown()
        {
            _bossIsHere.DOFade(0, .2f)
                .OnComplete(() => _bossIsHere.gameObject.SetActive(false));
            _root.SetActive(false);
        }

        StartCoroutine(
            Coroutines.WaitFor(seconds: 2, 
                callback: OnWritingShown));
    }

    private void ShowAppearingAnimation()
    {
        _bossIsHere.alpha = 0;
        _bossIsHere.DOFade(1, .2f);
    }
}
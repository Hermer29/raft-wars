using System;
using System.Collections;
using DG.Tweening;
using RaftWars.Infrastructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossAppearing : MonoBehaviour
{
    private const float TimeToGenerateBoss = 20f;
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
        StartCoroutine(QueryBossSpawnProcess());
    }

    private IEnumerator QueryBossSpawnProcess()
    {
        yield return null;
        _root.GetComponent<CanvasGroup>().alpha = 1;
        _slider.value = 1;
        _slider.DOValue(0, TimeToGenerateBoss)
            .OnComplete(OnComplete);
    }

    private void OnDisable()
    {
        _slider.DOKill();
    }

    private void OnComplete()
    {
        _generateBoss?.Invoke();
        _generateBoss = null;
        _root.GetComponent<CanvasGroup>().alpha = 0;
        _bossIsHere.gameObject.SetActive(true);
        _bossIsHere.alpha = 0;
        _bossIsHere.DOFade(1, .2f);
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
}
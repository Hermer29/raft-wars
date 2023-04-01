using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvas;
    [SerializeField] private Slider _slider;

    private const float SliderChangeSpeed = .5f;
    
    private float _targetSliderT;
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        _canvas.interactable = true;
        _canvas.blocksRaycasts = true;
    }

    private void Update()
    {
        _slider.value = Mathf.MoveTowards(_slider.value, _targetSliderT, Time.deltaTime * SliderChangeSpeed);
    }

    public void FadeIn()
    {
        StartCoroutine(ProcessCoroutine(1, (process) =>
        {
            _canvas.alpha = process;
        }));
        _canvas.interactable = true;
        _canvas.blocksRaycasts = true;
    }

    public void SetSliderProcess(float t)
    {
        _targetSliderT = t;
    }

    private IEnumerator ProcessCoroutine(float time, Action<float> process)
    {
        float startTime = Time.time;
        float endTime = startTime + time;
        while (startTime <= endTime)
        {
            process.Invoke(Mathf.InverseLerp(startTime, endTime, Time.time));
            yield return null;
        }
        process.Invoke(1);
    }

    public void FadeOut()
    {
        StartCoroutine(ProcessCoroutine(1, (process) =>
        {
            _canvas.alpha = 1 - process;
        }));
        _canvas.interactable = false;
        _canvas.blocksRaycasts = false;
    }
}

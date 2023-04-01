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
    }
}

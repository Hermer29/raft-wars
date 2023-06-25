using Interface.RewardWindows;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VIPOfferWindow : MonoBehaviour
{
    [SerializeField] private Button offerBtn;
    [SerializeField] public Button loseBtn;
    [SerializeField] public Button popUp;
    public event Action onCLose;
    public event Action onShowOffer;
    private void Start()
    {
        offerBtn.onClick.AddListener(ShowOffer);
        loseBtn.onClick.AddListener(Close);
        popUp.onClick.AddListener(Close);
    }
    public void Show()
    {
        
    }
    private void ShowOffer()
    {
        onShowOffer?.Invoke();
    }
    public void Close()
    {
        onCLose?.Invoke();
        Destroy(gameObject);
    }
}

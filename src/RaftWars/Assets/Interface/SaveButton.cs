using System;
using RaftWars.Infrastructure.Services;
using SpecialPlatforms;
using UnityEngine;
using UnityEngine.UI;

namespace Interface
{
    [RequireComponent(typeof(Button))]
    public class SaveButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private void Start()
        {
            _button.onClick.AddListener(() => AllServices.GetSingle<SaveService>().Save());
        }
    }
}
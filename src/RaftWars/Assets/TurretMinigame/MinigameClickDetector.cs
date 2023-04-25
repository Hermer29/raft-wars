using System;
using Interface;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TurretMinigame
{
    public class MinigameClickDetector : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TurretMinigameHud _hud;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                Clicked();
            }
        }

        private void Clicked()
        {
            _hud.OnStartClicked();
            Destroy(gameObject);
        }
    }
}
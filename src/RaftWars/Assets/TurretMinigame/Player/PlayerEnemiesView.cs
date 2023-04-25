using System;
using UnityEngine;
using UnityEngine.UI;

namespace TurretMinigame.Player
{
    public class PlayerEnemiesView : MonoBehaviour
    {
        [SerializeField] private Slider _health;
        [SerializeField] private Slider _enemies;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void ShowHealth(float percent)
        {
            _health.value = percent;
        }

        public void ShowEnemies(float percent)
        {
            _enemies.value = percent;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
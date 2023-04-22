using System;
using UnityEngine;

namespace TurretMinigame.Player
{
    public class MinigameTurret : MonoBehaviour
    {
        private TurretMinigameFactory _factory;
        
        [SerializeField] private Transform _tower;
        [SerializeField] private Vector3 _eulerRotationOffset;
        
        public void Construct(TurretMinigameFactory factory)
        {
            _factory = factory;
        }
        
        public void StartShooting()
        {
            
        }

        public void Rotate(float delta)
        {
            _tower.rotation =
                Quaternion.RotateTowards(_tower.rotation, Quaternion.LookRotation(transform.right), delta);
        }
    }
}
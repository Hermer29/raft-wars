using Cinemachine;
using TurretMinigame.Player;
using UnityEngine;

namespace TurretMinigame
{
    public class MinigamePlatform : MonoBehaviour
    {
        public CinemachineVirtualCamera PlayingCamera;
        public CinemachineVirtualCamera LookingAtTurretCamera;
        public Transform TurretSpawnPoint;
        
        public void PlaceTurret(MinigameTurret turretMinigame)
        {
            turretMinigame.transform.position = TurretSpawnPoint.position;
        }
    }
}
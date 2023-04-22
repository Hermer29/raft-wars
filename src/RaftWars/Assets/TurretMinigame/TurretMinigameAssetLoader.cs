using TurretMinigame.Player;
using UnityEngine;

namespace TurretMinigame
{
    public class TurretMinigameAssetLoader
    {
        private const string TurretsPath = "MinigameTurrets/Turret";
        private const string MinigamePlatformPath = "MinigamePlatform";
        private const string BulletPath = "Prefabs/Bullet";
        
        public MinigameTurret LoadTurret(int tier)
        {
            return Resources.Load<MinigameTurret>(TurretsPath + tier);
        }

        public MinigamePlatform LoadPlatform()
        {
            return Resources.Load<MinigamePlatform>(MinigamePlatformPath);
        }

        public GameObject LoadBullet()
        {
            return Resources.Load<GameObject>(BulletPath);
        }
    }
}
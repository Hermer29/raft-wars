using TurretMinigame.Player;
using UnityEngine;

namespace TurretMinigame
{
    public class TurretMinigameAssetLoader
    {
        private const string TurretsPath = "MinigameTurrets/Turret";
        private const string MinigamePlatformPath = "MinigamePlatform";
        private const string BulletPath = "Prefabs/MinigameBullet";
        private const string TurretMinigameHudPath = "Prefabs/UI/TurretMinigameHud";
        
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

        public TurretMinigameHud LoadTurretMinigameHud()
        {
            return Resources.Load<TurretMinigameHud>(TurretMinigameHudPath);
        }

        public bool ExistsTurretWithIndex(int playersTurretIndex)
        {
            return Resources.Load<MinigameTurret>(TurretsPath + playersTurretIndex) != null;
        }
    }
}
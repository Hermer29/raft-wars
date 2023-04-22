using TurretMinigame.Player;
using UnityEngine;

namespace TurretMinigame
{
    public class TurretMinigameFactory
    {
        private readonly TurretMinigameAssetLoader _loader;

        public TurretMinigameFactory(TurretMinigameAssetLoader loader)
        {
            _loader = loader;
        }

        public MinigamePlatform CreateMinigamePlatform()
        {
            return Object.Instantiate(_loader.LoadPlatform());
        }

        public MinigameTurret CreateTurret(int tier)
        {
            return Object.Instantiate(_loader.LoadTurret(tier));
        }

        public GameObject CreateBullet()
        {
            return Object.Instantiate(_loader.LoadBullet());
        }
    }
}
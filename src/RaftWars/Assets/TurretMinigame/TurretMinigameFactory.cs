using TurretMinigame.Player;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.Object;

namespace TurretMinigame
{
    public class TurretMinigameFactory
    {
        private readonly TurretMinigameAssetLoader _loader;
        private readonly ObjectPool<GameObject> _bullets;

        public TurretMinigameFactory(TurretMinigameAssetLoader loader)
        {
            _loader = loader;
            _bullets = new ObjectPool<GameObject>(
                createFunc: InstantiateBullet,
                defaultCapacity: 5, 
                actionOnRelease: obj =>
                {
                    obj.SetActive(false);
                },
                actionOnGet: obj =>
                {
                    obj.SetActive(true);
                });
        }

        private GameObject InstantiateBullet()
        {
            return Instantiate(_loader.LoadBullet());
        }

        public MinigamePlatform CreateMinigamePlatform()
        {
            return Instantiate(_loader.LoadPlatform());
        }

        public MinigameTurret CreateTurret(int tier)
        {
            return Instantiate(_loader.LoadTurret(tier));
        }

        public GameObject CreateBullet()
        {
            return _bullets.Get();
        }

        public void FreeBullet(GameObject bullet)
        {
            _bullets.Release(bullet);
        }

        public TurretMinigameHud CreateTurretMinigameHud()
        {
            return Instantiate(_loader.LoadTurretMinigameHud());
        }

        public bool TryCreateTurretOfTier(int tier, out MinigameTurret turret)
        {
            MinigameTurret minigameTurret = _loader.LoadTurret(tier);
            if (minigameTurret != null)
            {
                turret = minigameTurret;
                return true;
            }

            turret = null;
            return false;
        }
    }
}
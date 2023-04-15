using UnityEngine;

namespace Common
{
    public abstract class FighterRaft : MonoBehaviour
    {
        // TODO: Перенести всё касающееся боя сюда

        public abstract void Die();
        public abstract void StopFight();

        public abstract void AddPlatform(Platform platform);
        public abstract void AddTurret(Turret turret);
        public abstract void AddFastTurret(Turret turret);
        public abstract int PlatformsCount {get;}

        public void AddAbstractPlatform(Platform platform, Material color)
        {
            if (platform.isTurret)
            {
                var turret = platform.GetComponentInChildren<Turret>();
                turret.DrawInMyColor(color);
                if (turret.isWind)
                    AddFastTurret(turret);
                else
                    AddTurret(turret);
                    
            }
            AddPlatform(platform);
        }

        public abstract EnemyHud GetHud();

        public abstract void DealDamage(int damage = 1);
        public abstract Platform GetAnotherPlatform();
    }
}
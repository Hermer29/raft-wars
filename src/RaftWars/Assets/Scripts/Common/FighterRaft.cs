using SpecialPlatforms;
using UnityEngine;

namespace Common
{
    public abstract class FighterRaft : MonoBehaviour
    {
        // TODO: Перенести всё касающееся боя сюда

        public abstract void Die();
        public abstract void StopFight();

        public abstract void AddPlatform(Platform platform);
        public abstract void AddDamage(Turret turret, IDamageAmplifying statsHolder);
        public abstract void AddSpeed(Turret turret, ISpeedIncreasing statsHolder);
        public abstract int PlatformsCount {get;}
        public abstract int Damage { get; }
        public abstract int Health { get; }
        public abstract float MoveSpeed { get; }

        public void AddAbstractPlatform(Platform platform, Material color)
        {
            if (platform.isTurret)
            {
                var turret = platform.GetComponentInChildren<Turret>();
                var stats = platform.GetComponent<StatsHolder>();
                
                turret.DrawInMyColor(color);
                if (stats.Platform  is ISpeedIncreasing speedIncreasing)
                {
                    AddSpeed(turret, speedIncreasing);
                }
                else if (stats.Platform is IHealthIncreasing healthIncreasing)
                {
                    AddHealth(turret, healthIncreasing);
                }
                else if (stats.Platform is IDamageAmplifying damageAmplifying)
                {
                    AddDamage(turret, damageAmplifying);
                }
            }
            AddPlatform(platform);
        }

        public abstract void AddHealth(Turret turret, IHealthIncreasing stats);

        public abstract EnemyHud GetHud();

        public abstract void DealDamage(int damage = 1);
        public abstract Platform GetAnotherPlatform();
    }
}
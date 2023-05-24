using System;
using System.Collections.Generic;
using System.Linq;
using SpecialPlatforms;
using SpecialPlatforms.Concrete;
using Units;
using Unity.VisualScripting;
using UnityEngine;

namespace Common
{
    public abstract class FighterRaft : MonoBehaviour
    {
        private IEnumerable<SpecialPlatform> _specialPlatforms;
        private bool _useDefaultBalanceValues;
        public abstract void Die();
        public abstract void StopFight();

        public abstract int PlatformsCount {get;}
        public abstract int Damage { get; }
        public abstract int Health { get; }
        public abstract float MoveSpeed { get; }

        protected void Construct(IEnumerable<SpecialPlatform> platforms, bool useDefaultBalanceValues)
        {
            _useDefaultBalanceValues = useDefaultBalanceValues;
            _specialPlatforms = platforms;
        }
        
        public void AddAbstractPlatform(Platform platform, Material color)
        {
            if (platform.isTurret)
            {
                var turret = platform.GetComponentInChildren<Turret>();
                var stats = platform.GetComponent<StatsHolder>();

                if(turret != null)
                {
                    AddTurret(turret);
                    turret.DrawInMyColor(color);
                }

                if (stats.Platform  is ISpeedIncreasing)
                {
                    AddSpeedForPlatformType(stats.Platform.GetType());
                }
                else if (stats.Platform is IHealthIncreasing)
                {
                    AddHealthForPlatformType(stats.Platform.GetType());
                }
                else if (stats.Platform is IDamageAmplifyer)
                {
                    AddDamageForPlatformType(stats.Platform.GetType());
                }
                else if (stats.Platform is Magnet)
                {
                    AddMagnetWithPlatformType(stats.Platform.GetType(), turret);
                }
                else if (stats.Platform is Barracks)
                {
                    platform.AddComponent<BarracksSpawner>()
                        .Construct(
                            raft: this, 
                            balanceData: FindPlatformDataWithConcreteType<Barracks>(typeof(Barracks)), 
                            useDefaultValues: _useDefaultBalanceValues);
                }
            }
            AddPlatform(platform);
        }

        protected abstract void AddTurret(Turret turret);

        protected T FindPlatformDataWithConcreteType<T>(Type platform) where T : class
        {
            return _specialPlatforms.First(x => x.GetType() == platform) as T;
        }

        public abstract EnemyHud GetHud();

        public abstract void DealDamage(int damage = 1);
        public abstract Platform GetAnotherPlatform();
        protected abstract void AddHealthForPlatformType(Type data);
        protected abstract void AddDamageForPlatformType(Type data);
        protected abstract void AddSpeedForPlatformType(Type data);
        protected abstract void AddMagnetWithPlatformType(Type data, Turret turret);
        protected abstract void AddPlatform(Platform platform);

        public abstract bool TryGetNotFullPlatform(out Platform platform);
        public abstract (Vector3 position, Vector3 normal)[] GetOutsideNormals();
    }
}
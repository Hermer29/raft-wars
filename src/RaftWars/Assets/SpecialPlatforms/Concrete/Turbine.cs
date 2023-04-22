﻿using UnityEngine;

namespace SpecialPlatforms.Concrete
{
    [CreateAssetMenu(menuName = "Special platforms/🛬Create turbine")]
    public class Turbine : SpecialPlatform, ISpeedIncreasing
    {
        [field: Header("Platform specific")]
        [field: SerializeField] public float DefaultSpeedBonus { get; private set; }
        [SerializeField] private float _speedUpgradeStep;

        [field: SerializeField] public ValueType ValueType { get; private set; }
        public float SpeedBonus => DefaultSpeedBonus + UpgradedLevel * _speedUpgradeStep;
    }
}
using System;
using System.Collections;
using RaftWars.Infrastructure;
using UnityEngine;

namespace TurretMinigame.Service
{
    public class MinigameTurretInputService
    {
        private readonly ICoroutineRunner _coroutines;
        private float? _previousHorizontalPosition;

        public MinigameTurretInputService(ICoroutineRunner coroutines)
        {
            _coroutines = coroutines;
            
            _coroutines.StartCoroutine(ClockReadings());
        }

        public event Action<float> HorizontalDeltaPositionUpdated;

        private IEnumerator ClockReadings()
        {
            while (true)
            {
                if (Input.GetMouseButton(0) == false)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }

                float currentHorizontalPosition = Input.mousePosition.x;
                _previousHorizontalPosition ??= currentHorizontalPosition;
                float delta = currentHorizontalPosition - _previousHorizontalPosition.Value;
                if(delta != 0)
                    HorizontalDeltaPositionUpdated?.Invoke(delta);
                _previousHorizontalPosition = currentHorizontalPosition;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
using System.Collections;
using UnityEngine;

namespace RaftWars.Infrastructure
{
    public class Bootstrapper : MonoBehaviour, ICoroutineRunner
    {
        private static bool _exists;
        
        private void Awake()
        {
            if(_exists)
            {
                Destroy(gameObject);
                return;
            }

            _exists = true;
            DontDestroyOnLoad(gameObject);

            var loadingScreen = GameFactory.CreateLoadingScreen();
            var stateMachine = new StateMachine(this, loadingScreen);
            stateMachine.Enter<BootstrapState>();
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            base.StartCoroutine(coroutine);
        }
    }
}
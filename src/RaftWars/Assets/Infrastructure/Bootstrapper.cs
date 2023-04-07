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
            Execute();
        }

        private void Execute()
        {
            LoadingScreen loadingScreen = GameFactory.CreateLoadingScreen();
            loadingScreen.FadeInImmediately();
            var stateMachine = new StateMachine(this, loadingScreen);

            stateMachine.Enter<ProjectInitialization>();
            stateMachine.Enter<BootstrapState>();
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            base.StartCoroutine(coroutine);
        }
    }
}
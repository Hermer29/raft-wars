using System.Collections;
using Infrastructure.States;
using RaftWars.Infrastructure.Services;
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

            AllServices.Register<ICoroutineRunner>(this);
            stateMachine.Enter<ProjectInitialization>();
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            base.StartCoroutine(coroutine);
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RaftWars.Infrastructure
{
    public class LevelLauncher : MonoBehaviour
    {
        [SerializeField] private LoadingScreen _loading;

        private void Start()
        {
            LoadLevel();
            DontDestroyOnLoad(gameObject);
        }

        private void LoadLevel()
        {
            StartCoroutine(Load());
        }

        private IEnumerator Load()
        {
            var asyncOperation = SceneManager.LoadSceneAsync(1);
            while (asyncOperation.isDone == false)
            {
                _loading.SetSliderProcess(asyncOperation.progress);
                yield return null;
            }
            _loading.SetSliderProcess(1);
            yield return new WaitForSeconds(.5f);
            _loading.FadeOut();
        }
    }
}
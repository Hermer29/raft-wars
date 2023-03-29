using UnityEngine;
using UnityEngine.SceneManagement;

namespace RaftWars.Infrastructure
{
    public class BootstrapRedirector : MonoBehaviour
    {
        private void Awake()
        {
            if (Game.Initialized == false)
            {
                SceneManager.LoadScene("Bootstrap");
            }
        }
    }
}
using UnityEngine;

namespace RaftWars.Infrastructure
{
    public class Bootstrapper : MonoBehaviour
    {
        private void Awake()
        {
            var game = new Game();
            DontDestroyOnLoad(gameObject);
        }
    }
}
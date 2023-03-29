using UnityEngine;

namespace RaftWars.Infrastructure
{
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private Player _playerService;
        [SerializeField] private Camera _camera;
        
        private void Awake()
        {
            var game = new Game(_playerService, _camera);
            DontDestroyOnLoad(gameObject);
        }
    }
}
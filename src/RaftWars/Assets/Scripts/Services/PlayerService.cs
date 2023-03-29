using UnityEngine;

namespace InputSystem
{
    public class PlayerService
    {
        private Player _player;
        private readonly Camera _camera;

        public PlayerService(Player player, Camera camera)
        {
            _player = player;
            _camera = camera;
        }

        public float PlayerStatsSum => _player.damage + _player.hp;

        public Vector3 Position => _player.transform.position;
        public bool GameStarted => _player.canPlay;

        public bool IsInViewportBounds(Vector3 worldPoint, float viewportOffset)
        {
            Vector3 point = _camera.WorldToViewportPoint(worldPoint);
            return point.x <= 1 - viewportOffset && point.y <= 1 - viewportOffset && point.x >= 0 + viewportOffset && point.x >= 0 + viewportOffset;
        }
    }
}
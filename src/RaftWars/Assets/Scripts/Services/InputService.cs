using UnityEngine;

namespace InputSystem
{
    public class InputService : MonoBehaviour
    {
        private Joystick _joystick;
        private Player _player;

        public float Vertical 
            => Mathf.Clamp(_joystick.Vertical + UnityEngine.Input.GetAxis("Vertical"), -1, 1);

        public float Horizontal
            => Mathf.Clamp(_joystick.Horizontal + UnityEngine.Input.GetAxis("Horizontal"), -1, 1);

        private void Start()
        {
            _joystick = FindObjectOfType<Joystick>();
        }
    }
}
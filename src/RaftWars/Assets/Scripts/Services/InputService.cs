using UnityEngine;

namespace InputSystem
{
    public class InputService
    {
        private Joystick _joystick;

        public InputService(Joystick joystick)
        {
            _joystick = joystick;
        }
        
        public float Vertical 
            => Mathf.Clamp(_joystick.Vertical + UnityEngine.Input.GetAxis("Vertical"), -1, 1);

        public float Horizontal
            => Mathf.Clamp(_joystick.Horizontal + UnityEngine.Input.GetAxis("Horizontal"), -1, 1);

        public void Disable()
        {
            _joystick.gameObject.SetActive(false);
        }

        public void Enable()
        {
            _joystick.gameObject.SetActive(true);
        }
    }
}
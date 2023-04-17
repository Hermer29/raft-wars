using UnityEngine;

namespace InputSystem
{
    public class InputService
    {
        private Joystick _joystick;
        private bool _disabled;

        public InputService(Joystick joystick)
        {
            _joystick = joystick;
        }
        
        public float Vertical 
        {
            get
            {
                if(_disabled)
                    return 0;
                return Mathf.Clamp(_joystick.Vertical + UnityEngine.Input.GetAxis("Vertical"), -1, 1);
            }
        }

        public float Horizontal
        {
            get
            {
                if(_disabled)
                    return 0;
                return Mathf.Clamp(_joystick.Horizontal + UnityEngine.Input.GetAxis("Horizontal"), -1, 1);
            }
        }

        public void Disable()
        {
            _disabled = true;
            _joystick.gameObject.SetActive(false);
        }

        public void Enable()
        {
            _disabled = false;
            _joystick.gameObject.SetActive(true);
        }
    }
}
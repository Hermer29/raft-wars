using System;
using UnityEngine;
using UnityEngine.UI;

namespace input
{
    public class PlayerController : MonoBehaviour
    {
        private Joystick _joystick;
        private Player _player;

        public float Vertical 
            => Mathf.Clamp(_joystick.Vertical + Input.GetAxis("Vertical"), -1, 1);

        public float Horizontal
            => Mathf.Clamp(_joystick.Horizontal + Input.GetAxis("Horizontal"), -1, 1);

        private void Start()
        {
            _joystick = FindObjectOfType<Joystick>();
        }
    }
}
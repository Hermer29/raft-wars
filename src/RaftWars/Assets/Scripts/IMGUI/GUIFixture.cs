using System;
using InputSystem;
using RaftWars.Infrastructure;
using UnityEngine;

namespace DefaultNamespace.IMGUI
{
    public class GUIFixture : MonoBehaviour
    {
        private PlayerService _player;
        private bool _enabled;

        [SerializeField] private GUIStyle _style;
        [SerializeField] private Vector2 _viewportBtnPosition;
        [SerializeField] private Vector2 _viewPortBtnSize;
        
        private void Start()
        {
            _player = Game.PlayerService;
        }

        private void OnGUI()
        {
            var position = new Vector2(Screen.width * _viewportBtnPosition.x, Screen.height * _viewportBtnPosition.y);
            float height = Screen.height / _viewPortBtnSize.y;
            var size = new Vector2(Screen.width / _viewPortBtnSize.x, height);
            if(GUI.Button(new Rect(position, size), "Add People", _style))
            {
                _player.AddPeople();
            }

            Vector2 nextButtonPosition = position + new Vector2(0, -(height / 2) - height);
            if (GUI.Button(new Rect(nextButtonPosition, size), "Double player speed", _style))
            {
                _player.DoubleSpeed();
            }
            
        }
        
    }
}
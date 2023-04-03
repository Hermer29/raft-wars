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
        
        private void Start()
        {
            _player = Game.PlayerService;
        }

        private void OnGUI()
        {
            var position = new Vector2(Screen.width * 0.1f, Screen.height * 0.1f);
            var size = new Vector2(Screen.width / 5, Screen.height / 10);
            if(GUI.Button(new Rect(position, size),"Add People"))
            {
                _player.AddPeople();
            }  
        }
        
    }
}
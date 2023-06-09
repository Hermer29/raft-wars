﻿using System;
using System.Linq;
using Infrastructure;
using Infrastructure.States;
using InputSystem;
using RaftWars.Infrastructure;
using Services;
using UnityEngine;

namespace DefaultNamespace.IMGUI
{
    public class GUIFixture : MonoBehaviour
    {
        private PlayerService _player;
        private PlayerMoneyService _moneyService;
        private bool _enabled;

        [SerializeField] private GUIStyle _style;
        [SerializeField] private Vector2 _viewportBtnPosition;
        [SerializeField] private Vector2 _viewPortBtnSize;
        [Header("Jump lvl:")]
        [SerializeField] private int _level;

        [NaughtyAttributes.Button]
        public void Jump()
        {
            Game.StateMachine.Enter<LoadLevelState, int>(_level);
        }

        private void Start()
        {
            _player = Game.PlayerService;
            _moneyService = Game.MoneyService;
        }

        private void OnGUI()
        {
            var position = new Vector2(Screen.width * _viewportBtnPosition.x, Screen.height * _viewportBtnPosition.y);
            float height = Screen.height / _viewPortBtnSize.y;
            var size = new Vector2(Screen.width / _viewPortBtnSize.x, height);
            if(GUI.Button(new Rect(position, size), "Add 10/10", _style))
            {
                _player.Amplify(10);
            }

            Vector2 nextButtonPosition = position - new Vector2(0, (height / 2) + height);
            if (GUI.Button(new Rect(nextButtonPosition, size), "Double player speed", _style))
            {
                _player.DoubleSpeed();
            }
            
            Vector2 nextNextButtonPosition = nextButtonPosition - new Vector2(0, (height / 2) + height);
            if (GUI.Button(new Rect(nextNextButtonPosition, size), "Add 300 coins", _style))
            {
                _moneyService.AddCoins(300);
            }
            
            Vector2 nextNextNextButtonPosition = nextNextButtonPosition - new Vector2(0, (height / 2) + height);
            if (GUI.Button(new Rect(nextNextNextButtonPosition, size), "NextStage", _style))
            {
                Game.GameManager.NextStage();
            }

            foreach (Enemy enemy in FindObjectsOfType<Enemy>())
            {
                var fromCamera = Camera.main.WorldToScreenPoint(enemy.platforms[0].transform.position);
                Vector2 positionOnScreen = fromCamera;
                if(GUI.Button(new Rect(positionOnScreen, size), "Kill", _style))
                {
                    enemy.Dead();
                }
            }
        }
    }
}
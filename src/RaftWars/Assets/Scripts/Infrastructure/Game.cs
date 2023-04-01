﻿using InputSystem;
using UnityEngine;

namespace RaftWars.Infrastructure
{
    internal class Game
    {
        public static CollectiblesService CollectiblesService;
        public static PlayerService PlayerService;
        public static MaterialsService MaterialsService;
        public static bool Initialized { get; private set; }
        
        public Game(Player player, Camera camera)
        {
            CollectiblesService = new CollectiblesService();
            PlayerService = new PlayerService(player, camera);
            MaterialsService = new MaterialsService();
            Initialized = true;
        }
    }
}
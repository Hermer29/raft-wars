using Cinemachine;
using Interface;
using UnityEngine;
using static RaftWars.Infrastructure.AssetManagement.AssetLoader;
using static UnityEngine.Object;

namespace RaftWars.Infrastructure
{
    public static class GameFactory
    {
        public static Explosion CreateExplosion()
        {
            return Instantiate(LoadExplosion());
        }

        public static Player CreatePlayer()
        {
            return Instantiate(LoadPlayer());
        }

        public static Hud CreateHud()
        {
            return Instantiate(LoadHud());
        }

        public static GameObject CreateIMGUI()
        {
            return Instantiate(LoadIMGUI());
        }

        public static MapGenerator CreateMapGenerator(int level)
        {
            return Instantiate(LoadLevelGenerator(level));
        }

        public static GameManager CreateGameManager()
        {
            return Instantiate(LoadGameManager());
        }

        public static LoadingScreen CreateLoadingScreen()
        {
            return Instantiate(LoadLoadingScreen());
        }

        public static GameObject CreatePlatformEdge()
        {
            return Instantiate(LoadPlatformEdge());
        }

        public static ParticleSystem CreateCornerWaves()
        {
            return Instantiate(LoadCornerWaves());
        }

        public static CinemachineVirtualCamera CreatePlayerVirtualCamera()
        {
            return Instantiate(LoadPlayerVirtualCamera());
        }

        public static EnemyHud CreateBossHud()
        {
            return Instantiate(LoadBossHud());
        }

        public static EnemyHud CreateEnemyHud()
        {
            return Instantiate(LoadEnemyHud());
        }

        public static Pause CreatePauseMenu()
        {
            return Instantiate(LoadPauseMenu());
        }

        public static Canvas CreateStatsCanvas()
        {
            return Instantiate(LoadStatsCanvas());
        }
    }
}
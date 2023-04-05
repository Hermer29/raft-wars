using Cinemachine;
using Interface;
using RaftWars.Infrastructure.AssetManagement;
using UnityEngine;

namespace RaftWars.Infrastructure
{
    public class GameFactory
    {
        public static Player CreatePlayer()
        {
            return GameObject.Instantiate(AssetLoader.LoadPlayer());
        }

        public static Hud CreateHud()
        {
            return GameObject.Instantiate(AssetLoader.LoadHud());
        }

        public static GameObject CreateIMGUI()
        {
            return GameObject.Instantiate(AssetLoader.LoadIMGUI());
        }

        public static MapGenerator CreateMapGenerator(int level)
        {
            return GameObject.Instantiate(AssetLoader.LoadLevelGenerator(level));
        }

        public static GameManager CreateGameManager()
        {
            return GameObject.Instantiate(AssetLoader.LoadGameManager());
        }

        public static LoadingScreen CreateLoadingScreen()
        {
            return GameObject.Instantiate(AssetLoader.LoadLoadingScreen());
        }

        public static GameObject CreatePlatformEdge()
        {
            return GameObject.Instantiate(AssetLoader.LoadPlatformEdge());
        }

        public static ParticleSystem CreateCornerWaves()
        {
            return GameObject.Instantiate(AssetLoader.LoadCornerWaves());
        }

        public static CinemachineVirtualCamera CreatePlayerVirtualCamera()
        {
            return GameObject.Instantiate(AssetLoader.LoadPlayerVirtualCamera());
        }
    }
}
using Cinemachine;
using DefaultNamespace;
using Interface;
using UnityEngine;

namespace RaftWars.Infrastructure.AssetManagement
{
    public static class AssetLoader
    {
        public static Player LoadPlayer()
        {
            return Resources.Load<Player>(AssetConstants.PlayerPath);
        }

        public static Hud LoadHud()
        {
            return Resources.Load<Hud>(AssetConstants.HudPath);
        }

        public static FeatureFlags LoadFeatureFlags()
        {
            return Resources.Load<FeatureFlags>(AssetConstants.FeatureFlagsPath);
        }

        public static GameObject LoadIMGUI()
        {
            return Resources.Load<GameObject>(AssetConstants.IMGUIPath);
        }

        public static MapGenerator LoadLevelGenerator(int level)
        {
            Debug.Log("Trying to load "+ AssetConstants.CreateLevelGeneratorPath(level));
            return Resources.Load<MapGenerator>(AssetConstants.CreateLevelGeneratorPath(level));
        }

        public static GameManager LoadGameManager()
        {
            return Resources.Load<GameManager>(AssetConstants.GameManagerPath);
        }

        public static LoadingScreen LoadLoadingScreen()
        {
            return Resources.Load<LoadingScreen>(AssetConstants.LoadingScreenPath);
        }

        public static GameObject LoadPlatformEdge()
        {
            return Resources.Load<GameObject>(AssetConstants.EdgeResourcesPath);
        }

        public static ParticleSystem LoadCornerWaves()
        {
            return Resources.Load<ParticleSystem>(AssetConstants.CornerWavesResourcesPath);
        }

        public static CinemachineVirtualCamera LoadPlayerVirtualCamera()
        {
            return Resources.Load<CinemachineVirtualCamera>(AssetConstants.PlayerVirtualCameraPath);
        }
    }
}
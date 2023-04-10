using System.Collections.Generic;
using Cinemachine;
using DefaultNamespace;
using Interface;
using Skins;
using Skins.Hats;
using Skins.Platforms;
using UnityEngine;
using static RaftWars.Infrastructure.AssetManagement.AssetConstants;

namespace RaftWars.Infrastructure.AssetManagement
{
    public static class AssetLoader
    {
        public static Player LoadPlayer()
        {
            return Resources.Load<Player>(PlayerPath);
        }

        public static Hud LoadHud()
        {
            return Resources.Load<Hud>(HudPath);
        }

        public static FeatureFlags LoadFeatureFlags()
        {
            return Resources.Load<FeatureFlags>(FeatureFlagsPath);
        }

        public static GameObject LoadIMGUI()
        {
            return Resources.Load<GameObject>(IMGUIPath);
        }

        public static MapGenerator LoadLevelGenerator(int level)
        {
            Debug.Log("Trying to load "+ CreateLevelGeneratorPath(level));
            return Resources.Load<MapGenerator>(CreateLevelGeneratorPath(level));
        }

        public static GameManager LoadGameManager()
        {
            return Resources.Load<GameManager>(GameManagerPath);
        }

        public static LoadingScreen LoadLoadingScreen()
        {
            return Resources.Load<LoadingScreen>(LoadingScreenPath);
        }

        public static GameObject LoadPlatformEdge()
        {
            return Resources.Load<GameObject>(EdgeResourcesPath);
        }

        public static ParticleSystem LoadCornerWaves()
        {
            return Resources.Load<ParticleSystem>(CornerWavesResourcesPath);
        }

        public static CinemachineVirtualCamera LoadPlayerVirtualCamera()
        {
            return Resources.Load<CinemachineVirtualCamera>(PlayerVirtualCameraPath);
        }

        public static IEnumerable<PlatformSkin> LoadPlatformSkins()
        {
            return Resources.LoadAll<PlatformSkin>(PlatformSkinsPath);
        }

        public static IEnumerable<PlayerColors> LoadPlayerColors()
        {
            return Resources.LoadAll<PlayerColors>(PlayerColorsPath);
        }

        public static IEnumerable<HatSkin> LoadHatSkins()
        {
            return Resources.LoadAll<HatSkin>(HatSkinsPath);
        }

        public static Explosion LoadExplosion()
        {
            return Resources.Load<Explosion>(ExplosionPath);
        }

        public static EnemyHud LoadBossHud()
        {
            return Resources.Load<EnemyHud>(BossHudPath);
        }

        public static EnemyHud LoadEnemyHud()
        {
            return Resources.Load<EnemyHud>(EnemyHudPath);
        }

        public static Material LoadGreyDeathMaterial()
        {
            return Resources.Load<Material>(GreyDeathMaterialPath);
        }

        public static Pause LoadPauseMenu()
        {
            return Resources.Load<Pause>(PauseMenuPath);
        }

        public static Canvas LoadStatsCanvas()
        {
            return Resources.Load<Canvas>(StatsCanvasPath);
        }
    }
}
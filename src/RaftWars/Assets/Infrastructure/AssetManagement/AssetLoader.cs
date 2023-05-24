using System.Collections.Generic;
using Cinemachine;
using DefaultNamespace;
using Interface;
using Monetization;
using Mono.Cecil;
using Skins;
using Skins.Hats;
using Skins.Platforms;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static RaftWars.Infrastructure.AssetManagement.AssetConstants;

namespace RaftWars.Infrastructure.AssetManagement
{
    public static class AssetLoader
    {
        public static Player LoadPlayer() => Resources.Load<Player>(PlayerPath);

        public static Hud LoadHud() => Resources.Load<Hud>(HudPath);

        public static FeatureFlags LoadFeatureFlags() => Resources.Load<FeatureFlags>(FeatureFlagsPath);

        public static GameObject LoadIMGUI() => Resources.Load<GameObject>(IMGUIPath);

        public static AsyncOperationHandle<GameObject> LoadLevelGenerator(int level)
        {
            string levelPath = CreateLevelGeneratorPath(level);
            Debug.Log($"Trying to load {levelPath}");
            return Addressables.LoadAssetAsync<GameObject>(levelPath);
        }

        public static GameManager LoadGameManager() => Resources.Load<GameManager>(GameManagerPath);

        public static LoadingScreen LoadLoadingScreen() => Resources.Load<LoadingScreen>(LoadingScreenPath);

        public static GameObject LoadPlatformEdge() => Resources.Load<GameObject>(EdgeResourcesPath);

        public static ParticleSystem LoadCornerWaves() => Resources.Load<ParticleSystem>(CornerWavesResourcesPath);

        public static CinemachineVirtualCamera LoadPlayerVirtualCamera() => Resources.Load<CinemachineVirtualCamera>(PlayerVirtualCameraPath);

        public static IEnumerable<PlatformSkin> LoadPlatformSkins() => Resources.LoadAll<PlatformSkin>(PlatformSkinsPath);

        public static IEnumerable<PlayerColors> LoadPlayerColors() => Resources.LoadAll<PlayerColors>(PlayerColorsPath);

        public static IEnumerable<HatSkin> LoadHatSkins() => Resources.LoadAll<HatSkin>(HatSkinsPath);

        public static Explosion LoadExplosion() => Resources.Load<Explosion>(ExplosionPath);

        public static EnemyHud LoadBossHud() => Resources.Load<EnemyHud>(BossHudPath);

        public static EnemyHud LoadEnemyHud() => Resources.Load<EnemyHud>(EnemyHudPath);

        public static Material LoadGreyDeathMaterial() => Resources.Load<Material>(GreyDeathMaterialPath);

        public static Pause LoadPauseMenu() => Resources.Load<Pause>(PauseMenuPath);

        public static Canvas LoadStatsCanvas() => Resources.Load<Canvas>(StatsCanvasPath);

        public static Material LoadPlayerDeathMaterial() => Resources.Load<Material>(PlayerDeathMaterialPath);

        public static Tutorial LoadTutorial() => Resources.Load<Tutorial>(TutorialPath);

        public static AudioService LoadAudioService() => Resources.Load<AudioService>(AudioServicePath);

        public static People LoadPeople() => Resources.Load<People>(PeoplePath);

        public static PickingRaftPieceAdvertising LoadPickingRaftPieceAdvertising() 
            => Resources.Load<PickingRaftPieceAdvertising>(PickingRaftPieceAdvertisingPath);
    }
}
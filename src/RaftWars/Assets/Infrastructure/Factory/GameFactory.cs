using Cinemachine;
using Infrastructure;
using Interface;
using Interface.RewardWindows;
using LanguageChanger;
using Monetization;
using RaftWars.Infrastructure.AssetManagement;
using RaftWars.Pickables;
using SpecialPlatforms.SPRewardState;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Visual;
using static RaftWars.Infrastructure.AssetManagement.AssetLoader;
using static UnityEngine.Object;

namespace RaftWars.Infrastructure
{
    public class GameFactory
    {
        private readonly ICoroutineRunner _runner;

        public GameFactory(ICoroutineRunner runner)
        {
            _runner = runner;
        }
        
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

        public AsyncOperationHandle<GameObject> CreateMapGenerator(int level)
        {
            return LoadLevelGenerator(level);
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
            return Instantiate(LoadEnemyHud());
        }

        public static EnemyHud CreateStatsHud()
        {
            return Instantiate(LoadEnemyHud(), Game.StatsCanvas.transform, false);
        }

        public static PickingRaftPieceAdvertising CreateRaftPieceAdvertising(AttachablePlatform platform, TextName platformName)
        {
            PickingRaftPieceAdvertising element =  Instantiate(
                original: LoadPickingRaftPieceAdvertising(), 
                parent: Game.StatsCanvas.transform, 
                worldPositionStays: false);
            platform.MakeNotPickableNormally();
            element.GetComponent<TransformBindUI>().Target = platform.transform;
            element.Construct(platform, platformName);
            var presenceTrigger = platform.AddComponent<PickableNearbyPresenceTrigger>();
            presenceTrigger.Construct(element);
            presenceTrigger.Initialize();
            return element;
        }

        public static Pause CreatePauseMenu()
        {
            return Instantiate(LoadPauseMenu());
        }

        public static Canvas CreateStatsCanvas()
        {
            return Instantiate(LoadStatsCanvas());
        }

        public static Tutorial CreateTutorial()
        {
            return Instantiate(LoadTutorial());
        }

        public static AudioService CreateAudioService()
        {
            var instance = Instantiate(LoadAudioService());
            instance.Construct(CrossLevelServices.PrefsService);
            return instance;
        }

        public static SPRewardWindow CreateSPRewardWindow()
        {
            return Instantiate(Resources.Load<SPRewardWindow>("SPRewardWindow"));
        }

        public static RandomRewardWindow CreateRewardWindow()
        {
            return Instantiate(Resources.Load<RandomRewardWindow>("RandomRewardWindow"));
        }
        public static VIPOfferWindow CreateVIPOfferWindow()
        {
            return Instantiate(Resources.Load<VIPOfferWindow>("Prefabs/UI/VIPOfferPopUp"));
        }
    }
}
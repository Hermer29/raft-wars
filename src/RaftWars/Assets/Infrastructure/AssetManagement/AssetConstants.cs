namespace RaftWars.Infrastructure.AssetManagement
{
    public static class AssetConstants
    {
        public const string LoadingScreenPath = "Prefabs/LoadingScreen";
        public const string GameManagerPath = "Prefabs/GameManager";
        public const string PlayerPath = "Prefabs/Player";
        public const string HudPath = "Prefabs/Hud";
        public const string FeatureFlagsPath = "FeatureFlags";
        public const string IMGUIPath = "Prefabs/IMGUI";
        public const string CornerWavesResourcesPath = "Prefabs/CornerWaves";
        public const string EdgeResourcesPath = "Prefabs/Edge";
        public const string PlayerVirtualCameraPath = "Prefabs/PlayerVirtualCamera";
        public const string PlatformSkinsPath = "Skins/PlatformModels";
        public const string HatSkinsPath = "Skins/PeopleHat";
        public const string PlayerColorsPath = "Skins/PlayerColors";
        public const string ExplosionPath = "Prefabs/Explosion";
        public const string BossHudPath = "Prefabs/BossHud";
        public const string EnemyHudPath = "Prefabs/EnemyHud";
        public const string GreyDeathMaterialPath = "DiedPeopleMaterial";
        public const string PauseMenuPath = "Prefabs/UI/PauseMenu";
        public const string StatsCanvasPath = "Prefabs/UI/StatsCanvas";
        public const string PlayerDeathMaterialPath = "PlayerColorOnDeath";
        public const string TutorialPath = "Prefabs/UI/Tutorial";
        public const string AudioServicePath = "Prefabs/AudioService";
        public const string PeoplePath = "Prefabs/People";
        public const string PickingRaftPieceAdvertisingPath = "Prefabs/UI/AdvertisingSpecialPlatform";

        public static string CreateLevelGeneratorPath(int level)
        {
            return $"Generators/Level{level}";
        }

    }
}
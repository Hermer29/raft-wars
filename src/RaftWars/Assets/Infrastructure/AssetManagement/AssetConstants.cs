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

        public static string CreateLevelGeneratorPath(int level)
        {
            return $"Generators/Level{level}";
        }
    }
}
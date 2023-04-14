using RaftWars.Infrastructure.Services;
using DefaultNamespace;

namespace RaftWars.Infrastructure
{
    public class CrossLevelServices
    {
        public static LevelService LevelService;
        public static IPrefsService PrefsService;
        
        public CrossLevelServices(ICoroutineRunner coroutineRunner, FeatureFlags featureFlags)
        {
            switch(featureFlags.PrefsImplementation)
            {
                case PrefsOptions.YandexCloud:
                    PrefsService = new YandexPrefsService(coroutineRunner);
                    break;
                case PrefsOptions.PlayerPrefs:
                    PrefsService = new PlayerPrefsService(coroutineRunner);
                    break;
            }
            LevelService = new LevelService(PrefsService);
        }
    }
}
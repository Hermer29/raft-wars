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
            LevelService = new LevelService(PrefsService);
        }
    }
}
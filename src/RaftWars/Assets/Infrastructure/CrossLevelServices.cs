using RaftWars.Infrastructure.Services;

namespace RaftWars.Infrastructure
{
    public class CrossLevelServices
    {
        public static LevelService LevelService;
        public static IPrefsService PrefsService;
        
        public CrossLevelServices(ICoroutineRunner coroutineRunner)
        {
            PrefsService = new PlayerPrefsService(coroutineRunner);
            LevelService = new LevelService(PrefsService);
        }
    }
}
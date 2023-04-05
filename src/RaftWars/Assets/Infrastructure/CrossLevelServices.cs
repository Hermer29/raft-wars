using RaftWars.Infrastructure.Services;

namespace RaftWars.Infrastructure
{
    public class CrossLevelServices
    {
        public static LevelService LevelService;
        public static IPrefsService PrefsService;
        
        public CrossLevelServices()
        {
            PrefsService = new PlayerPrefsService();
            LevelService = new LevelService(PrefsService);
        }
    }
}
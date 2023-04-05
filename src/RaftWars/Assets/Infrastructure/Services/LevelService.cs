namespace RaftWars.Infrastructure.Services
{
    public class LevelService
    {
        private readonly IPrefsService _prefsService;
        
        private const string LevelPrefsKey = "Level";

        public LevelService(IPrefsService prefsService)
        {
            _prefsService = prefsService;
        }
        
        public int Level
        {
            get
            {
                int level = _prefsService.GetInt(LevelPrefsKey, defaultValue: 1);
                if (level <= 0)
                    level = 1;
                return level;
            }
            private set => _prefsService.SetInt(LevelPrefsKey, value);
        }

        public void Increment()
        {
            Level++;
        }
    }
}
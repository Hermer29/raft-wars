using DefaultNamespace.Skins;
using RaftWars.Infrastructure.Services;

namespace Services
{
    public class PropertyService
    {
        private readonly IPrefsService _prefsService;

        public PropertyService(IPrefsService prefsService)
        {
            _prefsService = prefsService;
        }

        public bool IsOwned(IOwnable ownable)
        {
            if (_prefsService.HasKey(ownable.Guid) == false)
                return false;
            
            return _prefsService.GetInt(ownable.Guid) != 0;
        }

        public void Own(IOwnable ownable)
        {
            _prefsService.SetInt(ownable.Guid, 1);
        }
    }
}
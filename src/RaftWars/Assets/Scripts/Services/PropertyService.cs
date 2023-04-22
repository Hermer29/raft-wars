using System;
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

        public event Action<IAcquirable> PropertyOwned;

        public bool IsOwned(IAcquirable acquirable)
        {
            if (acquirable.OwnedByDefault)
                return true;
            
            if (_prefsService.HasKey(ConstructKey(acquirable)) == false)
                return false;
            
            return _prefsService.GetInt(ConstructKey(acquirable), defaultValue: 0) == 1;
        }

        public void Own(IAcquirable acquirable)
        {
            _prefsService.SetInt(ConstructKey(acquirable), 1);
            PropertyOwned?.Invoke(acquirable);
        }

        private string ConstructKey(IAcquirable acquirable)
        {
            const string PropertyServiceSuffix = "_PropertyService";
            return acquirable.Guid + PropertyServiceSuffix;
        }
    }
}
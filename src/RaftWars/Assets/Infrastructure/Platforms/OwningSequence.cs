using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Services;

namespace Infrastructure.Platforms
{
    public class OwningSequence<T> where T: ISequentiallyOwning
    {
        private readonly PropertyService _propertyService;
        private readonly Dictionary<int, ISequentiallyOwning> _sequence = new();
        private readonly FeatureFlags _flags;

        public OwningSequence(PropertyService propertyService, FeatureFlags flags)
        {
            _propertyService = propertyService;
            _flags = flags;
        }
        
        public void Register(ISequentiallyOwning sequentiallyOwning)
        {
            if (_flags.OwningOrderDefinition == false)
                return;
            _sequence.Add(sequentiallyOwning.Serial, sequentiallyOwning);
        }
        
        public bool IsCanBeOwned(ISequentiallyOwning sequentiallyOwning)
        {
            if (_flags.OwningOrderDefinition == false)
                return true;
            if (_sequence.ContainsKey(sequentiallyOwning.Serial) == false)
                throw new InvalidOperationException();
            foreach (var owning in _sequence.OrderBy(X => X.Value.Serial))
            {
                if (_propertyService.IsOwned(owning.Value) && owning.Value.Guid != sequentiallyOwning.Guid)
                {
                    continue;
                }

                if (sequentiallyOwning.Guid == owning.Value.Guid)
                {
                    return true;
                }

                if (_propertyService.IsOwned(owning.Value) == false)
                {
                    return false;
                }
            }

            return false;
        }
    }
}
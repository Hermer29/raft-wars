using System;
using System.Collections.Generic;
using System.Linq;
using Services;

namespace Infrastructure.Platforms
{
    public class OwningSequence<T> where T: ISequentiallyOwning
    {
        private readonly PropertyService _propertyService;
        private readonly Dictionary<int, ISequentiallyOwning> _sequence = new();

        public OwningSequence(PropertyService propertyService)
        {
            _propertyService = propertyService;
        }
        
        public void Register(ISequentiallyOwning sequentiallyOwning)
        {
            _sequence.Add(sequentiallyOwning.Serial, sequentiallyOwning);
        }
        
        public bool IsCanBeOwned(ISequentiallyOwning sequentiallyOwning)
        {
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
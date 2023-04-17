using System.Collections.Generic;
using System.Linq;

namespace DefaultNamespace
{
    public static class RandomExtensions
    {
        public static TElement Random<TElement>(this IEnumerable<TElement> source)
        {
            return source.ElementAt(UnityEngine.Random.Range(0, source.Count()));
        }
    }
}
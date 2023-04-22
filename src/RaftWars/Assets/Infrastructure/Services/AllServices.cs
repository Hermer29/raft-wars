using System;

namespace RaftWars.Infrastructure.Services
{
    public static class AllServices
    {
        public static void Register<TService>(TService instance)
        {
            KekwDictionary<TService>.value = instance;
        }

        public static TService GetSingle<TService>()
        {
            if (KekwDictionary<TService>.value == null)
            {
                throw new InvalidOperationException();
            }

            return KekwDictionary<TService>.value;
        }

        private static class KekwDictionary<T>
        {
            public static T value;
        }
    }
    
}
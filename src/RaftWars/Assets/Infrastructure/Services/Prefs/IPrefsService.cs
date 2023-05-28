using System;

namespace RaftWars.Infrastructure.Services
{
    public interface IPrefsService
    {
        public bool IsDataLoaded { get; }
        public event Action DataJustLoaded;
        string GetString(string key);
        void SetString(string key, string value);
        void SetInt(string key, int value);
        int GetInt(string key, int defaultValue = 0);
        bool HasKey(string key);
    }
}
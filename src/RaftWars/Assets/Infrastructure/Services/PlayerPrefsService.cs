using UnityEngine;

namespace RaftWars.Infrastructure.Services
{
    public class PlayerPrefsService : IPrefsService
    {
        public string GetString(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }

        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }
    }
}
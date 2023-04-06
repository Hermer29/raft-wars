using System;
using Agava.YandexGames;

namespace RaftWars.Infrastructure.Services
{

    public class YandexPrefsService : IPrefsService
    {
        public YandexPrefsService()
        {
            
        }

        public string GetString(string key)
        {
            PlayerAccount.GetPlayerData((result) =>
            {
                
            });
            throw new NotImplementedException();
        }

        public void SetString(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void SetInt(string key, int value)
        {
            throw new NotImplementedException();
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            throw new NotImplementedException();
        }

        public bool HasKey(string key)
        {
            throw new NotImplementedException();
        }
    }
}
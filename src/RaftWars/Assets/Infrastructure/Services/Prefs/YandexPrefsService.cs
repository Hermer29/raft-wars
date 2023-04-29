using System;
using System.Collections;
using System.Collections.Generic;
using Agava.YandexGames;
using Newtonsoft.Json;
using UnityEngine;

namespace RaftWars.Infrastructure.Services
{

    public class YandexPrefsService : IPrefsService
    {
        private Dictionary<string, string> _data;

        public YandexPrefsService(ICoroutineRunner coroutineRunner)
        {
            PlayerAccount.GetCloudSaveData((result) =>
            {
                _data = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            });
            coroutineRunner.StartCoroutine(SaveOverTime());
        }

        private IEnumerator SaveOverTime()
        {
            var serialized = JsonConvert.SerializeObject(_data);
            PlayerAccount.SetCloudSaveData(serialized);
            yield return new WaitForSeconds(2f);
        }

        public string GetString(string key)
        {
            return _data[key];
        }

        public void SetString(string key, string value)
        {
            _data[key] = value;
        }

        public void SetInt(string key, int value)
        {
            _data[key] = value.ToString();
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            if(_data.ContainsKey(key) == false)
                return defaultValue;
            return int.Parse(_data[key]);
        }

        public bool HasKey(string key)
        {
            return _data.ContainsKey(key);
        }
    }
}
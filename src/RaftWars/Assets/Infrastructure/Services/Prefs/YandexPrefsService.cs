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

        public YandexPrefsService(ICoroutineRunner coroutineRunner, Action onLoaded)
        {
            PlayerAccount.GetCloudSaveData((result) =>
            {
                InitializeService(result);
                onLoaded?.Invoke();
                coroutineRunner.StartCoroutine(SaveOverTime());
            });
        }

        private void InitializeService(string json)
        {
            if(string.IsNullOrEmpty(json))
            {
                _data = new Dictionary<string, string>();
                return;
            }
            _data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json, new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Include
            });
            if(_data == null)
                _data = new Dictionary<string, string>();
        }

        private void CreateDataIfEqualsNull()
        {
            if(_data == null)
                _data = new Dictionary<string, string>();
        }

        private IEnumerator SaveOverTime()
        {
            while(true)
            {
                if(_data.Count == 0)
                {
                    yield return null;
                    continue;
                }
                var serialized = JsonConvert.SerializeObject(_data);
                PlayerAccount.SetCloudSaveData(serialized);
                yield return new WaitForSeconds(1.5f);
            } 
        }

        public string GetString(string key)
        {
            CreateDataIfEqualsNull();
            if(HasKey(key) == false)
                return String.Empty;
            return _data[key];
        }

        public void SetString(string key, string value)
        {
            CreateDataIfEqualsNull();
            _data[key] = value;
        }

        public void SetInt(string key, int value)
        {
            CreateDataIfEqualsNull();
            _data[key] = value.ToString();
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            CreateDataIfEqualsNull();
            if(_data.ContainsKey(key) == false)
                return defaultValue;
            return int.Parse(_data[key]);
        }

        public bool HasKey(string key)
        {
            CreateDataIfEqualsNull();
            return _data.ContainsKey(key);
        }
    }
}
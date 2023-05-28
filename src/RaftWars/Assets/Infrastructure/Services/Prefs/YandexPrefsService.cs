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
        private readonly ICoroutineRunner _coroutineRunner;
        private Dictionary<string, string> _data;
        private int _version;

        public YandexPrefsService(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
            
            coroutineRunner.StartCoroutine(Worker());
        }

        private void Initialize() => PlayerAccount.GetCloudSaveData(InitializeService);

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

        private IEnumerator Worker()
        {
            Initialize();
            var version = _version;
            while(true)
            {
                if(_data.Count == 0)
                {
                    yield return null;
                    continue;
                }

                if (_version == version)
                {
                    yield return null;
                    continue;
                }

                version = _version;
                string serialized = JsonConvert.SerializeObject(_data);
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
            _version++;
        }

        public void SetInt(string key, int value)
        {
            CreateDataIfEqualsNull();
            _data[key] = value.ToString();
            _version++;
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
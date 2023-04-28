using UnityEngine;

namespace Editor
{
    [CreateAssetMenu]
    public class LevelSelector : ScriptableObject
    {
        [SerializeField] private int _level;
        
        #pragma warning disable
        
        [NaughtyAttributes.Button]
        public void Jump()
        { 
            PlayerPrefs.SetInt("Level", _level);
        }
        
        #pragma warning restore
    }
}
namespace RaftWars.Infrastructure.Services
{
    public interface IPrefsService
    {
        string GetString(string key);
        void SetString(string key, string value);
        void SetInt(string key, int value);
        int GetInt(string key, int defaultValue = 0);
        bool HasKey(string key);
    }
}
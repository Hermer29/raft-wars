namespace SpecialPlatforms
{
    public interface ISaveData
    {
        string GetData();
        string Key();
        void Populate(string data);
    }
}
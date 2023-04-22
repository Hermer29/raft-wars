namespace SpecialPlatforms
{
    public interface ISavableData
    {
        string GetData();
        string Key();
        void Populate(string data);
    }
}
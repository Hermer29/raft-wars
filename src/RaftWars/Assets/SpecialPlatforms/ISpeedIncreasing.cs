namespace SpecialPlatforms
{
    public interface ISpeedIncreasing
    {
        ValueType ValueType { get; }
        float SpeedBonus { get; }
        float DefaultSpeedBonus { get; }
    }
}
namespace SpecialPlatforms
{
    public interface IHealthIncreasing
    {
        ValueType ValueType { get; }
        float HealthValue { get; }
        float DefaultHealthGain { get; }
    }
}
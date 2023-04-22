namespace SpecialPlatforms
{
    public interface IDamageAmplifying
    {
        ValueType ValueType { get; }
        float DamageValue { get; }
        float BaseDamage { get; }
    }
}
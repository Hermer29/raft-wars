namespace SpecialPlatforms
{
    public interface IDamageAmplifyer
    {
        ValueType ValueType { get; }
        float DamageValue { get; }
        float BaseDamage { get; }
    }
}
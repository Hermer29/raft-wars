namespace Units
{
    public interface IPurchasableSpecialPlatform
    {
        string ProductIDForUpgrade { get; }
        string ProductIDForAcquirement { get; }
    }
}
namespace Interface.RewardWindows
{
    public interface IRewardWindow<TData>
    {
        void Show(TData data);
    }
}
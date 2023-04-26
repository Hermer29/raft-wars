using System;
using RaftWars.Infrastructure.Services;

namespace Services
{
    public class PlayerMoneyService
    {
        private readonly IPrefsService _prefsService;

        private const string MoneyKey = "Money";

        public PlayerMoneyService(IPrefsService prefsService)
        {
            _prefsService = prefsService;
        }

        public int Amount => _prefsService.GetInt(MoneyKey);

        public event Action<int> AmountUpdated;

        public void Spend(int amount)
        {
            int previous = _prefsService.GetInt(MoneyKey);
            if (previous < amount)
            {
                throw new InvalidOperationException("Insufficient funds");
            }
            _prefsService.SetInt(MoneyKey, previous - amount);
            AmountUpdated?.Invoke(Amount);
        }

        public void AddCoins(int coins)
        {
            int previous = _prefsService.GetInt(MoneyKey);
            _prefsService.SetInt(MoneyKey, previous + coins);
            AmountUpdated?.Invoke(Amount);
        }

        public bool HasEnoughCoins(int amount)
        {
            int previous = _prefsService.GetInt(MoneyKey);
            return previous >= amount;
        }

        public bool TrySpendCoins(int amount)
        {
            if (HasEnoughCoins(amount) == false)
                return false;
            
            Spend(amount);
            return true;
        }
    }
}
using System;
using RaftWars.Infrastructure.Services;

namespace InputSystem
{
    public class PlayerMoneyService
    {
        private readonly IPrefsService _prefsService;
        
        private const string MoneyKey = "Money";

        public PlayerMoneyService(IPrefsService prefsService)
        {
            _prefsService = prefsService;
        }

        public void Spend(int amount)
        {
            int previous = _prefsService.GetInt(MoneyKey);
            if (previous < amount)
            {
                throw new InvalidOperationException("Insufficient coins");
            }
            _prefsService.SetInt(MoneyKey, previous - amount);
        }

        public void AddCoins(int coins)
        {
            int previous = _prefsService.GetInt(MoneyKey);
            _prefsService.SetInt(MoneyKey, previous + coins);
        }

        public bool HasEnoughCoins(int amount)
        {
            int previous = _prefsService.GetInt(MoneyKey);
            return previous > amount;
        }
    }
}
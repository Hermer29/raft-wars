using System;
using Interface;
using RaftWars.Infrastructure.Services;

namespace InputSystem
{
    public class PlayerMoneyService
    {
        private readonly IPrefsService _prefsService;
        private readonly Hud _hud;

        private const string MoneyKey = "Money";

        public PlayerMoneyService(IPrefsService prefsService, Hud hud)
        {
            _prefsService = prefsService;
            _hud = hud;

            _hud.coinsText.text = Amount.ToString();
        }

        public int Amount => _prefsService.GetInt(MoneyKey);

        public void Spend(int amount)
        {
            int previous = _prefsService.GetInt(MoneyKey);
            if (previous < amount)
            {
                throw new InvalidOperationException("Insufficient coins");
            }
            _prefsService.SetInt(MoneyKey, previous - amount);
            _hud.coinsText.text = Amount.ToString();
        }

        public void AddCoins(int coins)
        {
            int previous = _prefsService.GetInt(MoneyKey);
            _prefsService.SetInt(MoneyKey, previous + coins);
            _hud.coinsText.text = Amount.ToString();
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
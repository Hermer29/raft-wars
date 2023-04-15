using System;

public class Damageable
{
    private int _hp;
    private int _damage;
    private Action _onChanged;

    public Damageable(ref int hp, ref int damage, Action onChanged)
    {
        _hp = hp;
        _damage = damage;
        _onChanged = onChanged;
    }

    public void DealDamage(int amount = 1)
    {
        
    }

    public void StartFight()
    {

    }

    public void StopFight()
    {

    }
}
using UnityEngine;

interface ICanTakePeople
{
    public bool TryTakePeople(GameObject warrior);
}

interface ICanTakePlatform
{
    public void TakePlatform(GameObject platform, Vector3 pos);
}

public interface ICoroutineSender
{
    public void CoroutineDone();
}

public interface ICanTakeGems
{
    public void TakeGems(int gems);
}

public interface ICanTakeCoins
{
    public bool TryTakeCoins(int coins);
}

public interface ICanTakeBarrel
{
    public bool TryTakeBarrel(int damage);
}

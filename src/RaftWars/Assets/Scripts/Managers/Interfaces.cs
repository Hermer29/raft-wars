using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface ICanTakePeople
{
    public void TakePeople(GameObject warrior);
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
    public void TakeCoins(int coins);
}

public interface ICanTakeBarrel
{
    public void TakeBarrel(int damage);
}

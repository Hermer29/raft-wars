using System.Collections;
using System.Collections.Generic;
using RaftWars.Infrastructure;
using UnityEngine;

public class Coins : MonoBehaviour
{

    public int coins;
    private bool canTake = true;
    public GameObject effect;

    private void OnTriggerEnter(Collider other)
    {
        var coinsTaker = other.GetComponent<ICanTakeCoins>();
        if (coinsTaker != null && canTake)
        {
            if (coinsTaker.TryTakeCoins(coins))
            {
                canTake = false;
                GameObject _effect = Instantiate(effect, transform.position, Quaternion.identity);
                Destroy(_effect, 2f);
                Destroy(gameObject);
                Game.AudioService.CoinPickedUp();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{

    public int coins;
    private bool canTake = true;
    public GameObject effect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ICanTakeCoins>() != null && canTake)
        {
            if (canTake)
            {
                canTake = false;
                GameObject _effect = Instantiate(effect, transform.position, Quaternion.identity);
                Destroy(_effect, 2f);
                other.GetComponent<ICanTakeCoins>().TakeCoins(coins);
                Destroy(gameObject);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public int damage;
    private bool canTake = true;
    public GameObject effect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ICanTakeBarrel>() != null && canTake)
        {
            if (canTake)
            {
                canTake = false;
                GameObject _effect = Instantiate(effect, transform.position, Quaternion.identity);
                Destroy(_effect, 2f);
                other.GetComponent<ICanTakeBarrel>().TakeBarrel(damage);
                Destroy(gameObject);
            }
        }
    }
}

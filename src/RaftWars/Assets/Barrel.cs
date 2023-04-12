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
        var barrelTaker = other.GetComponent<ICanTakeBarrel>();
        if (barrelTaker != null && canTake)
        {
            if (barrelTaker.TryTakeBarrel(damage))
            {
                canTake = false;
                GameObject _effect = Instantiate(effect, transform.position, Quaternion.identity);
                Destroy(_effect, 2f);
                Destroy(gameObject);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gems : MonoBehaviour
{
    public int gems;
    private bool canTake = true;
    public GameObject effect;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ICanTakeGems>() != null && canTake)
        {
            if (canTake)
            {
                canTake = false;
                GameObject _effect = Instantiate(effect, transform.position, Quaternion.identity);
                Destroy(_effect, 2f);
                other.GetComponent<ICanTakeGems>().TakeGems(gems);
                Destroy(gameObject);
            }
        }
    }
}

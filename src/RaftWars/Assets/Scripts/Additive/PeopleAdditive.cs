using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleAdditive : MonoBehaviour
{
    public GameObject warrior;
    private bool canTake = true;
    public GameObject explosion;


    private void Update()
    {
        if (transform.position.x >= 45)
            transform.Translate(new Vector3(-5, 0, 0) * Time.deltaTime);
        else if (transform.position.x <= -45)
            transform.Translate(new Vector3(5, 0, 0) * Time.deltaTime);
        if (transform.position.z >= 45)
            transform.Translate(new Vector3(0, 0, -5) * Time.deltaTime);
        else if (transform.position.z <= -45)
            transform.Translate(new Vector3(0, 0, 5) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ICanTakePeople>() != null && canTake)
        {
            if (canTake)
            {
                canTake = false;
                GameObject _exp = Instantiate(explosion, transform.position, Quaternion.identity);
                Destroy(_exp, 2);
                other.GetComponent<ICanTakePeople>().TakePeople(warrior);
                Destroy(gameObject);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAdditive : MonoBehaviour
{
    public GameObject platform;

    private bool canTake = true;

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
        if (other.GetComponent<ICanTakePlatform>() != null && canTake)
        {
            canTake = false;
            GetComponent<BoxCollider>().enabled = false;
            other.GetComponent<ICanTakePlatform>().TakePlatform(platform, transform.position);
            Destroy(gameObject);
            
        }
    }
}

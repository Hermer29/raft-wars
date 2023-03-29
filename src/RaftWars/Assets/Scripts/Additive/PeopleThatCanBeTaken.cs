using UnityEngine;
using UnityEngine.Serialization;

public class PeopleThatCanBeTaken : MonoBehaviour
{
    public GameObject warrior;
    private bool canTake = true;
    [FormerlySerializedAs("explosion")] public GameObject explosionPrefab;
    
    private void Update()
    {
        Wander();
    }

    private void Wander()
    {
        switch (transform.position.x)
        {
            case >= 45:
                transform.Translate(new Vector3(-5, 0, 0) * Time.deltaTime);
                break;
            case <= -45:
                transform.Translate(new Vector3(5, 0, 0) * Time.deltaTime);
                break;
        }

        switch (transform.position.z)
        {
            case >= 45:
                transform.Translate(new Vector3(0, 0, -5) * Time.deltaTime);
                break;
            case <= -45:
                transform.Translate(new Vector3(0, 0, 5) * Time.deltaTime);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ICanTakePeople otherTaker) == false || !canTake) return;
        
        canTake = false;
        CreateExplosion();
        otherTaker.TakePeople(warrior);
        Destroy(gameObject);
    }

    private void CreateExplosion()
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, 2);
    }
}

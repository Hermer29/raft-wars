using System;
using RaftWars.Pickables;
using UnityEngine;
using UnityEngine.Serialization;

public class PeopleThatCanBeTaken : Pickable
{
    public GameObject warrior;
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

    protected override void TriggerEntered(Collider other)
    {
        if (other.TryGetComponent(out ICanTakePeople otherTaker) == false || !canTake) return;
        if (!otherTaker.TryTakePeople(warrior)) return;
        canTake = false;
        CreateExplosion();
        Destroy(gameObject);
    }

    private void CreateExplosion()
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, 2);
    }
}

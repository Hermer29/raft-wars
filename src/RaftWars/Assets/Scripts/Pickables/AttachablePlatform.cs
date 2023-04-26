using System;
using Infrastructure;
using RaftWars.Infrastructure;
using RaftWars.Pickables;
using UnityEngine;

public class AttachablePlatform : Pickable
{
    public GameObject platform;

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
    
    protected override void TriggerEntered(Collider other)
    {
        if (other.TryGetComponent<ICanTakePlatform>(out var otherTaker) == false || !canTake) return;
        canTake = false;
        if(otherTaker is Platform {isEnemy: false})
        {
            Game.AudioService.PlayPlatformPickingUpSound();
        }
        GetComponent<BoxCollider>().enabled = false;
        other.GetComponent<ICanTakePlatform>().TakePlatform(platform, transform.position);
        Destroy(gameObject);
    }
}

using Infrastructure;
using RaftWars.Pickables;
using UnityEngine;

public class AttachablePlatform : Pickable
{
    public GameObject platform;
    private bool _notPickable;
    private ICanTakePlatform _lastTaker;

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
        if (CantTakePlatform(other, out ICanTakePlatform otherTaker)) return;
        if (_notPickable)
        {
            _lastTaker = otherTaker;
            return;
        }
        Take(otherTaker);
    }

    private bool CantTakePlatform(Collider other, out ICanTakePlatform otherTaker) 
        => other.TryGetComponent(out otherTaker) == false || !canTake;

    internal void Take(ICanTakePlatform by, bool andDestroy = true)
    {
        canTake = false;
        if (by is Platform { isEnemy: false })
        {
            Game.AudioService.PlayPlatformPickingUpSound();
        }
        GetComponent<BoxCollider>().enabled = false;
        by.TakePlatform(platform, transform.position);
        Destroy(gameObject);
    }

    public void MakeNotPickableNormally() => _notPickable = true;
}

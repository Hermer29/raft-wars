using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 _target;
    private Vector3 shotDirection;

    private float timer = 0;
    private const float AccuracyError = 5f;
    private float speed = 10;

    public GameObject explosion;

    public void MoveTowards(Vector3 target)
    {
        _target = target;
        _target = AddAccuracyError(_target);
        shotDirection = (_target - transform.position).normalized;
    }

    private static Vector3 AddAccuracyError(Vector3 target) 
        => target + new Vector3(Random.Range(-AccuracyError, AccuracyError), 0, 
            Random.Range(-AccuracyError, AccuracyError));

    private void Update()
    {
        transform.position += shotDirection * (speed * Time.deltaTime);
        if (timer > Random.Range(0.2f, 0.6f))
        {
            Destroy(gameObject);
        }
        else
            timer += Time.deltaTime;
    }
}

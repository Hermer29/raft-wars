using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlyCamera : MonoBehaviour
{
    [SerializeField] private float speed = 5;

    private bool shouldMove = false;

    private Vector3 dir;

    private void Update()
    {
        if (shouldMove)
        {
            transform.DOLocalMove(dir, 3);
        }
    }

    public void Move()
    {
        shouldMove = true;
        dir = (transform.localPosition - transform.forward * 4);
    }
}

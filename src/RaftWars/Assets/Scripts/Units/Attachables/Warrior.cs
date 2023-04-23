using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayAttackAnim(Transform target)
    {
        transform.LookAt(target, Vector3.up);
        anim.Play("Shoot");
    }

    public void PlayDeathAnim()
    {
        anim.Play("Death" + Random.Range(0, 3));
        Destroy(gameObject, 2);
    }

    public void PlayIdle()
    {
        anim.Play("Idle");
    }
}

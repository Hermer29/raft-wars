using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class People : MonoBehaviour
{
    public bool isAdditive;

    public SkinnedMeshRenderer matRenderer;
    private Animator animator;

    public Bullet bullet;
    public Transform shootPoint;

    private bool battle;
    private float timer = 0;
    private Vector3 target;
    private bool isDead = false;
    public bool isFlamer = false;
    public GameObject flame;
    public AudioSource audio;
    private Vector3 targetShoot;
    public GameObject shootEffect;

    private void Update()
    {
        if (!battle || isDead) return;
        if (timer >= Random.Range(0.6f, 1f))
        {
            timer = 0;
            animator.Play("Shoot");
            if (!isFlamer)
            {
                PerformShot();
            }
            else
            {
                EnableFlamethrower();
            }
        }
        else
            timer += Time.deltaTime;
    }

    private void EnableFlamethrower()
    {
        flame.SetActive(true);
        StartCoroutine(Coroutines.WaitFor(1.3f, () => { flame.SetActive(false); }));
    }

    private void PerformShot()
    {
        Destroy(Instantiate(shootEffect, shootPoint.position, Quaternion.identity), 2);
        Bullet _bullet = Instantiate(bullet, shootPoint.position, Quaternion.identity);
        _bullet.MoveTowards(targetShoot);
        audio.Play();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null && isAdditive)
        {
            animator.Play("Hold");
        }
    }

    public void SetColor(Material mat)
    {
        matRenderer.material = mat;
    }

    public void PlayShotAnimation(Transform target)
    {
        this.target = target.position;
        this.target.y = transform.position.y;
        targetShoot = this.target + new Vector3(0, 1, 0);
        transform.LookAt(this.target, Vector3.up);
        battle = true;
        animator.Play("Idle Aiming");
    }

    public void IdleAnim()
    {
        animator.Play("Idle");
        battle = false;
    }

    public void DeathAnim()
    {
        isDead = true;
        animator.Play("Death");
        Destroy(gameObject, 2.5f);
    }
}

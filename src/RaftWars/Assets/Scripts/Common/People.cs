using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class People : MonoBehaviour
{
    public bool isAdditive;

    public SkinnedMeshRenderer matRenderer;
    private Animator anim;

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
        if (battle && !isDead)
        {
            if (timer >= Random.Range(0.6f, 1f))
            {

                    timer = 0;

                    anim.Play("Shoot");
                    if (!isFlamer)
                    {
                        Destroy(Instantiate(shootEffect, shootPoint.position, Quaternion.identity), 2);
                        Bullet _bullet = Instantiate(bullet, shootPoint.position, Quaternion.identity).GetComponent<Bullet>();
                        _bullet.SetPrefs(targetShoot);
                        audio.Play();
                    }
                    else
                    {
                        flame.SetActive(true);
                        StartCoroutine(Coroutines.WaitFor(1.3f, delegate ()
                        {
                            flame.SetActive(false);
                        }));
                    }
                
            }
            else
                timer += Time.deltaTime;
        }
    }

    private void Start()
    {
        anim = GetComponent<Animator>();

        if (anim != null && isAdditive)
        {
            anim.Play("Hold");
        }
    }

    public void SetColor(Material mat)
    {
        matRenderer.material = mat;
    }

    public void ShootAnim(Transform target)
    {
        this.target = target.position;
        this.target.y = transform.position.y;
        targetShoot = this.target + new Vector3(0, 1, 0);
        transform.LookAt(this.target, Vector3.up);
        battle = true;
        anim.Play("Idle Aiming");
    }

    public void IdleAnim()
    {
        anim.Play("Idle");
        battle = false;
    }

    public void DeathAnim()
    {
        isDead = true;
        anim.Play("Death");
        Destroy(gameObject, 2.5f);
    }

}

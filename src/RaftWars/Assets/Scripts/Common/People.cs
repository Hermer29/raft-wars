using System;
using System.Collections;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class People : MonoBehaviour
{
    public bool isAdditive;

    public SkinnedMeshRenderer matRenderer;
    public Animator animator;

    [FormerlySerializedAs("bullet")] public Bullet bulletPrefab;
    public Transform shootPoint;

    public bool battle;
    private float timer = 0;
    private Vector3 target;
    private bool isDead = false;
    public bool isFlamer = false;
    public GameObject flame;
    public AudioSource audio;
    private Vector3 targetShoot;
    public GameObject shootEffect;
    private Platform _platform;
    private Coroutine _movingOnPlatform;

    public Material Material
    {
        set => matRenderer.material = value;
    }

    private void OnValidate()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (animator != null && isAdditive)
        {
            animator.Play("Hold");
        }
    }

    private IEnumerator MoveOnPlatformOverTime()
    {
        const float speed = 1f;

        while (true)
        {
            const float platformSizeModifier = 5;
            Vector3 nextPoint = new Vector3(
                Random.Range(-Constants.PlatformSize / platformSizeModifier, Constants.PlatformSize / platformSizeModifier),
                    .5f,
                Random.Range(-Constants.PlatformSize / platformSizeModifier, Constants.PlatformSize / platformSizeModifier));
            transform.rotation = Quaternion.LookRotation(nextPoint - transform.localPosition);
            PlayRunAnimation();
            while (Vector3.SqrMagnitude(nextPoint - transform.localPosition) > Mathf.Epsilon)
            {
                if(battle)
                {
                    yield return null;
                    continue;
                }
                
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, nextPoint, speed * Time.deltaTime);
                yield return null;
            }
            IdleAnim();
            const float spacingBetweenMoves = 1f;
            yield return new WaitForSeconds(spacingBetweenMoves);
        }
    }

    public void SetRelatedPlatform(Platform platform)
    {
        _platform = platform;
        Material = platform.Material;
        transform.localScale = new Vector3(1 / platform.transform.lossyScale.x, 
            1 / platform.transform.lossyScale.y, 1 / platform.transform.lossyScale.z);
        _movingOnPlatform = StartCoroutine(MoveOnPlatformOverTime());
    }

    private void Update()
    {
        if (!battle || isDead) 
            return;
        if (timer >= Random.Range(0.6f, 1f))
        {
            timer = 0;
            animator.Play("Shoot");
            ShotWithFlamethrower();
            if (!isFlamer)
            {
                PerformShot();
            }
        }
        else
            timer += Time.deltaTime;
    }

    private void ShotWithFlamethrower()
    {
        flame.SetActive(true);
        StartCoroutine(Coroutines.WaitFor(1.3f, () => { flame.SetActive(false); }));
    }

    private void PerformShot()
    {
        Destroy(Instantiate(shootEffect, shootPoint.position, Quaternion.identity), 2);
        Bullet _bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        _bullet.MoveTowards(targetShoot);
        audio.Play();
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

    public void PlayRunAnimation()
    {
        animator.Play("Running");
    }

    public void IdleAnim()
    {
        if (animator != null)
        {
            animator.Play("Idle");
        }
        battle = false;
    }

    public void PlayDyingAnimation()
    {
        isDead = true;
        StopCoroutine(_movingOnPlatform);
        animator.Play("Death");
        Destroy(gameObject, 2.5f);
    }
}

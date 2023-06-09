using System.Collections;
using RaftWars.Infrastructure;
using RaftWars.Infrastructure.AssetManagement;
using Services;
using Skins.Hats;
using Units.Enemies;
using UnityEngine;
using UnityEngine.Assertions;
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
    public bool isDead = false;
    public bool isFlamer = false;
    public GameObject flame;
    public AudioSource audio;
    private Vector3 targetShoot;
    public GameObject shootEffect;
    private Platform _platform;
    private Coroutine _movingOnPlatform;
    public Transform HatPosition;
    private GameObject _hat;

    public Material Material
    {
        set 
        {
            if(value == null)
            {
                matRenderer.material = Resources.Load<Material>("Boss5StageMaterial");
                return;
            }
            matRenderer.material = value;
        }
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
            Vector3 nextPoint = Platform.GetRandomPoint(platformSizeModifier: 5);
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
        Assert.IsNotNull(platform);
        _platform = platform;
        Material = platform.Material;
        Transform transform1 = platform.transform;
        transform.localScale = new Vector3(1 / transform1.lossyScale.x, 
            1 / transform1.lossyScale.y, 1 / transform1.lossyScale.z);
        _movingOnPlatform = StartCoroutine(MoveOnPlatformOverTime());
    }

    private void Update()
    {
        if (!battle || isDead) 
            return;

        RotateTowardsTarget();
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

    public void PlayShotAnimation(ITargetable target)
    {
        this.target = target.GetRandomTarget();
        this.target.y = transform.position.y;
        targetShoot = this.target + Vector3.up;
        //transform.LookAt(target.position, Vector3.up);
        RotateTowardsTarget();
        battle = true;
        animator.Play("Idle Aiming");
    }

    private void RotateTowardsTarget()
    {
        transform.rotation = Quaternion.LookRotation(this.target - transform.position, Vector3.up);
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

    public void PlayDyingAnimation(bool andDestroy = true)
    {
        isDead = true;
        StopCoroutine(_movingOnPlatform);
        animator.Play("Death");
        //TODO: if(andDestroy)
        _platform.Capacity--;
        _platform = null;
        Destroy(gameObject, 2.5f);
    }

    public void ApplyHat(HatSkin hat)
    {
        if (_hat != null)
        {
            Destroy(_hat.gameObject);
        }

        if(hat != null)
            _hat = Instantiate(hat, HatPosition).gameObject;
    }

    public void MakeGrey()
    {
        Material = AssetLoader.LoadGreyDeathMaterial();
    }

    public void ThrowAway()
    {
        //TODO: StartCoroutine(ThrowProcess());
    }

    private IEnumerator ThrowProcess()
    {
        const float height = 3f;
        var randomRadian = Random.Range(0, 360) * Mathf.Deg2Rad;
        var randomDirection = new Vector3(Mathf.Sin(randomRadian), 0, Mathf.Cos(randomRadian));
        while (true)
        {
            
        }
    }
}

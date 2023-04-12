using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Turret : MonoBehaviour, ICoroutineSender
{
    public bool isAdditive;

    public int damageIncrease;
    public int healthIncrease;

    public Bullet bullet;
    public Transform shootPoint;
    public GameObject shootEffect;

    private bool battle;
    private float timer = 0;
    private Vector3 target;
    private bool isDead = false;

    public GameObject toRotate;
    private Coroutine coroutine;

    public bool isFlamer = false;
    public GameObject[] flame;

    public bool isWind = false, ishospital = false;
    public GameObject mill;
    public float millSpeed;
    private Vector3 targetShoot;

    public bool isLaser = false;
    public GameObject laser;

    public bool nonBattle = false;
    public bool isMagnet = false;

    private Collider[] cols;
    private Vector3 posToCast;
    LayerMask mask;

    public MeshRenderer[] objectsToDraw;

    private void Start()
    {
        mask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Water");   
    }

    private void Update()
    {
        if (!isWind && !ishospital && !nonBattle)
        {
            if (battle && !isDead)
            {
                toRotate.transform.LookAt(this.target, Vector3.up);
                if (!isLaser)
                {
                    if (timer >= Random.Range(1f, 1.5f))
                    {
                        if (Random.Range(0, 1f) > 0.2f)
                        {
                            timer = 0;
                            if (!isFlamer)
                            {
                                Bullet _bullet = Instantiate(bullet, shootPoint.position, Quaternion.identity).GetComponent<Bullet>();
                                _bullet.MoveTowards(targetShoot);
                                shootEffect.SetActive(true);
                                StartCoroutine(Coroutines.WaitFor(0.7f, delegate ()
                                {
                                    shootEffect.SetActive(false);
                                }));
                            }
                            else
                            {
                                for(int i = 0; i < flame.Length; i++)
                                {
                                    GameObject _flame = flame[i];
                                    _flame.SetActive(true);
                                    StartCoroutine(Coroutines.WaitFor(1.3f, delegate ()
                                    {
                                        _flame.SetActive(false);
                                    }));
                                }

                            }
                        }
                    }
                    else
                        timer += Time.deltaTime;
                }
            }
            else if (!battle && !isDead)
            {
                if (coroutine == null)
                {
                    coroutine = StartCoroutine(Coroutines.WaitFor(3, delegate ()
                    {
                        Vector3 rot = toRotate.transform.eulerAngles;
                        rot.y += Random.Range(-150f, 150f);
                        toRotate.transform.DORotate(rot, 1.5f);
                    },
                    this));
                }
            }
        }
        else
        {
            if (!battle && isWind)
            {
                mill.transform.RotateAround(Vector3.forward, 0.02f);
            }
        }
        if (isMagnet)
        {
            posToCast = transform.position;
            toRotate.transform.RotateAround(Vector3.up, 0.02f);
            cols = Physics.OverlapSphere(posToCast, 6, ~mask, QueryTriggerInteraction.Collide);
            if (cols != null)
            {
                foreach (Collider col in cols)
                {
                    if (col.gameObject.GetComponent<PeopleThatCanBeTaken>() != null)
                    {
                        col.transform.Translate((posToCast - col.transform.position) * Time.deltaTime);
                    }
                }
            }
        }
    }

    public void ShootAnim(Transform target)
    {
        if (!isWind && !ishospital && !nonBattle)
        {
            this.target = target.position;
            this.target.y = toRotate.transform.position.y;
            targetShoot = this.target + new Vector3(0, 1, 0);
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            toRotate.transform.LookAt(this.target, Vector3.up);
            battle = true;
            if (isLaser)
            {
                laser.SetActive(true);
            }
        }
    }

    public void IdleAnim()
    {
        battle = false;
        if (isLaser)
            laser.SetActive(false);
    }

    public void CoroutineDone()
    {
        coroutine = null;
    }

    public void DrawInMyColor(Material mat)
    {
        if (objectsToDraw.Length > 0)
        {
            foreach (MeshRenderer mesh in objectsToDraw)
            {
                mesh.material = mat;
            }
        }
    }
}

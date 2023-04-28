using UnityEngine;
using DG.Tweening;
using Units.Attachables;


public class Turret : MonoBehaviour, ICoroutineSender, IMagnetTurret
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
    public GameObject _magnetEffect;

    private Collider[] cols;
    private Vector3 posToCast;
    LayerMask mask;

    public MeshRenderer[] objectsToDraw;
    private float _radius = 6;

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
                                Bullet _bullet = Instantiate(bullet, shootPoint.position, Quaternion.identity)
                                    .GetComponent<Bullet>();
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
            if (!battle && isWind && mill != null)
            {
                mill.transform.RotateAround(Vector3.forward, 0.02f);
            }
        }
        if (isMagnet)
        {
            posToCast = transform.position;
            toRotate.transform.RotateAround(Vector3.up, 0.02f);
            cols = Physics.OverlapSphere(posToCast, _radius, ~mask, QueryTriggerInteraction.Collide);
            if (cols != null)
            {
                foreach (Collider col in cols)
                {
                    if (col.gameObject.GetComponent(typeof(IDraggableByMagnet)) != null)
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

    public void DrawInMyColor(Material material)
    {
        var visual = GetComponent<PlatformsVisual>();
        if (visual == null)
        {
            Debug.LogWarning($"[Turret] Unable to draw color on turret {name}");
            return;
        }
        visual.Colorize(material);
    }

    void IMagnetTurret.ModifyPickingSpace(float modifier)
    {
        _radius *= modifier;
        _magnetEffect.transform.localScale *= modifier;
    }
}
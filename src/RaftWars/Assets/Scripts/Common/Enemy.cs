using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DG.Tweening;
using InputSystem;
using RaftWars.Infrastructure;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using Visual;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;


public class Enemy : MonoBehaviour, IPlatformsCarrier
{
    [SerializeField] private TextMeshPro hpText;
    [SerializeField] private TextMeshPro damageText;
    [SerializeField] private TextMeshPro nickname;
    [SerializeField] public List<Platform> platforms = new List<Platform>();
    [FormerlySerializedAs("fullDamage")] public float maximumDamage;
    [SerializeField] private float hpIncrease = 5;
    [SerializeField] private float damageIncrease = 5;
    [SerializeField] private bool _disableEdges;
    private List<People> warriors = new List<People>();
    private List<Turret> turrets = new List<Turret>();
    private int warriorsCount;
    private float hpClear = 0;
    private float damageClear = 0;
    private float turretDamage = 0;
    private float currentHp = 0;
    [FormerlySerializedAs("fullHp")] public float maximumHp;
    public bool battle = false;
    private float speed = 5 - 5/4;
    private Player player;
    private float timer = 0;
    private const float RunningViewportBounds = .05f;
    public bool isDead;
    public bool isBoss = false;
    private List<PeopleThatCanBeTaken> peopleAdditive = new List<PeopleThatCanBeTaken>();
    private List<AttachablePlatform> platformsAdditive = new List<AttachablePlatform>();
    private Vector3 prevSpawnPoint;
    private PlayerService _player;
    public bool boss5Stage = true;
    private float playerDmg;
    public bool hasShield = false;
    public List<GameObject> enemiesToKill;
    public GameObject shieldToOff;
    private Vector3? _moveDirection;
    private bool _fleeingPlayer;
    public Material _material;
    private MaterialsService _materialService;
    private const float SqrMagnitudeDistanceToReactOnPlayer = 10 * 10;
    private const int ExclusionSqrDistanceToPlayer = 300;

    private int StatsSum => (int) (maximumHp + maximumDamage);

    public Material Material
    {
        set
        {
            _material = value;
            SetColor(_material);
        }
    }

    private void Start()
    {
        _player = Game.PlayerService;
        _materialService = Game.MaterialsService;
        TryGenerateNickname(when: !isBoss);
        GenerateRandomColor(when: isBoss && _material == null);
        WarmupEdges();
        Player.Died += OnPlayerDied;
        if (!boss5Stage) return;
        WarmupPlatforms();
        AssignRelatedPeople();
        RecountStats();
        
    }

    private void WarmupEdges()
    {
        if (_disableEdges)
            return;
        var edges = gameObject.AddComponent<EdgesAndAngleWaves>();
        edges.Construct(this, _material);
        edges.CreateEdges();
        edges.CreateWaves();
    }

    private void GenerateRandomColor(bool when)
    {
        if (!when)
            return;

        _material = _materialService.GetRandom();
        SetColor(_material);
    }

    private void SetColor(Material material)
    {
        foreach (People warrior in warriors)
        {
            warrior.matRenderer.material = material;
        }
    }

    private void AssignRelatedPeople()
    {
        foreach (People people in GetComponentsInChildren<People>())
        {
            platforms[0].TakePeople(people.gameObject);
        }
    }

    private void WarmupPlatforms()
    {
        foreach (Platform platform in platforms)
        {
            platform.isEnemy = true;
            platform.gameObject.layer = LayerMask.NameToLayer("Enemy");
            if (!platform.isTurret) continue;
            turrets.Add(platform.GetComponentInChildren<Turret>());
            turretDamage += platform.GetComponentInChildren<Turret>().damageIncrease;
            currentHp += platform.GetComponentInChildren<Turret>().healthIncrease;
        }
    }

    private void TryGenerateNickname(bool when)
    {
        if (!when)
            return;
        
        nickname.text = "Player" + Random.Range(1000, 10000);
    }

    private void Update()
    {
        if (hasShield)
        {
            for(int i = 0; i < enemiesToKill.Count; i++)
            {
                if (enemiesToKill[i] == null)
                    enemiesToKill.RemoveAt(i);
                if (enemiesToKill.Count != 0) continue;
                shieldToOff.SetActive(false);
                hasShield = false;
            }
        }

        if (!battle)
        {
            TryMoveEnemy(Time.deltaTime);
        }
        
        if (player != null && !player.isDead) return;
        battle = false;
        timer = 0;
    }

    private void TryMoveEnemy(float deltaTime)
    {
        if (_player.InBattle)
            return;
        if (_player.IsDead)
        { }
        else if (_player.GameStarted == false)
            return;

        if (isDead)
            return;

        if (_moveDirection == null)
        {
            MoveInRandomDirection();
        }

        bool ReachedWorldBounds()
        {
            return Vector3.Distance(GetNearestPointOnBound(), transform.position) <= 1f;
        }

        if (ReachedWorldBounds())
        {
            _moveDirection = Vector3.Reflect(_moveDirection.Value, GetNormalToNearestBound());
        }
        float sqrMagnitudeToPlayer = (_player.Position - transform.position).sqrMagnitude;
        var escapeVector = -(_player.Position - transform.position).normalized;
        
        if (sqrMagnitudeToPlayer < ExclusionSqrDistanceToPlayer)
        {
            if (_player.ExistsEnemyThatAlreadyInExclusionZone && _player.EnemyInExclusionZone != this)
            {
                _moveDirection = escapeVector;
            }
            else
            {
                _player.RegisterAsEnemyInExclusionZone(this);
            }
        }
        else if(_player.EnemyInExclusionZone == this)
        {
            _player.UnregisterEnemyInExclusionZone();
        }
        if(_player.IsDead)
        {}
        else if(sqrMagnitudeToPlayer < SqrMagnitudeDistanceToReactOnPlayer)
        {
            bool playerSuperior = _player.PlayerStatsSum >= StatsSum;
            if (playerSuperior)
            {
                if(ReachedWorldBounds())
                {
                    MoveInRandomDirection();
                    goto Exit;
                }

                _moveDirection = escapeVector;
            }
            else
            {
                _moveDirection = (_player.Position - transform.position).normalized;
            }
        }

        Exit:
        transform.position += _moveDirection.Value * (deltaTime * speed);
    }

    private void MoveInRandomDirection()
    {
        float radians = Random.Range(0, 360f) * Mathf.Deg2Rad;
        _moveDirection = new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));
    }

    private Vector3 GetNearestPointOnBound()
    {
        Vector3 position = transform.position;
        float distanceToZMaxBound = Mathf.Abs(45 - position.z);
        float distanceToZMinBound = Mathf.Abs(-45 - position.z);
        float distanceToXMinBound = Mathf.Abs(-45 - position.x);
        float distanceToXMaxBound = Mathf.Abs(45 - position.x);

        float minimal = Mathf.Min(distanceToZMaxBound, distanceToZMinBound, distanceToXMinBound, distanceToXMaxBound);
        Vector3 result = Vector3.zero;
        if (distanceToZMaxBound == minimal)
        {
            result = new Vector3(position.x, 0, 45);
        }

        if (distanceToZMinBound == minimal)
        {
            result = new Vector3(position.x, 0, -45);
        }

        if (distanceToXMinBound == minimal)
        {
            result = new Vector3(-45, 0, position.z);
        }

        if (distanceToXMaxBound == minimal)
        {
            result = new Vector3(45, 0, position.z);
        }

        return result;
    }

    private Vector3 GetNormalToNearestBound()
    {
        Vector3 position = transform.position;
        float distanceToZMaxBound = Mathf.Abs(45 - position.z);
        float distanceToZMinBound = Mathf.Abs(-45 - position.z);
        float distanceToXMinBound = Mathf.Abs(-45 - position.x);
        float distanceToXMaxBound = Mathf.Abs(45 - position.x);
        var minimal = Mathf.Min(distanceToZMaxBound, distanceToZMinBound, distanceToXMinBound, distanceToXMaxBound);
        if (distanceToZMaxBound == minimal)
        {
            return Vector3.back;
        }

        if (distanceToZMinBound == minimal)
        {
            return Vector3.forward;
        }

        if (distanceToXMinBound == minimal)
        {
            
            return Vector3.right;
        }

        if (distanceToXMaxBound == minimal)
        {
            return Vector3.left;
        }

        throw new Exception("Unreachable");
    }

    private void FixedUpdate()
    {
        if (battle)
        {
            GetDamage(playerDmg);
        }
    }

    public void SpawnEnvironment(IEnumerable<Platform> platforms, People[] people, int hp, int damage, List<AttachablePlatform> platformsAdd, List<PeopleThatCanBeTaken> peopleAdd)
    {
        if (boss5Stage) return;
        platformsAdditive = platformsAdd;
        peopleAdditive = peopleAdd;
        this.platforms[0].isEnemy = true;
        this.platforms[0].Material = _material;
        this.platforms[0].gameObject.layer = LayerMask.NameToLayer("Enemy");
        Vector3 startPoint = transform.position;
        hpIncrease = hp;
        damageIncrease = damage;
        warriorsCount = people.Length;
        prevSpawnPoint = startPoint;
        
        foreach (Platform platform in platforms)
        {
            startPoint = ThinkOutSpawnPosition(startPoint);
            Platform plat = Instantiate(platform, startPoint, Quaternion.identity, transform);
            plat.Material = _material;
            plat.isEnemy = true;
            plat.gameObject.layer = LayerMask.NameToLayer("Enemy");
            if (plat.ishospital)
            {
                currentHp += plat.GetComponentInChildren<Turret>().healthIncrease;
                maximumHp += plat.GetComponentInChildren<Turret>().healthIncrease;
            }
            else if (plat.isTurret)
            {
                turrets.Add(plat.GetComponentInChildren<Turret>());
                turretDamage += plat.GetComponentInChildren<Turret>().damageIncrease;
                maximumDamage += plat.GetComponentInChildren<Turret>().damageIncrease;
                currentHp += plat.GetComponentInChildren<Turret>().healthIncrease;
                maximumHp += plat.GetComponentInChildren<Turret>().healthIncrease;
                platform.GetComponentInChildren<Turret>().DrawInMyColor(_material);
            }
            else
                this.platforms.Add(plat);
        }

        foreach (People man in people)
        {
            this.platforms[Random.Range(0, this.platforms.Count)].TakePeople(man.gameObject);
        }

        RecountStats();
    }

    private Vector3 ThinkOutSpawnPosition(Vector3 startPoint)
    {
        int counter = 0;
        
        while (true)
        {
            counter++;
            if (counter == 10)
                break;
            if (Random.Range(0f, 1f) > 0.5f)
            {
                if (Random.Range(0f, 1f) > 0.5f)
                    startPoint.x += Constants.PlatformSize;
                else
                    startPoint.x -= Constants.PlatformSize;
            }
            else
            {
                if (Random.Range(0f, 1f) > 0.5f)
                    startPoint.z += Constants.PlatformSize;
                else
                    startPoint.z -= Constants.PlatformSize;
            }

            var outCols = Physics.OverlapSphere(startPoint, 1);
            if (outCols == null || outCols.Length == 0)
            {
                break;
            }

            startPoint = prevSpawnPoint;
        }

        return startPoint;
    }

    private void RecountStats()
    {
        hpText.text = Mathf.RoundToInt(maximumHp).ToString();
        damageText.text = Mathf.RoundToInt(maximumDamage).ToString();
    }

    public void AddPeople(People warrior)
    {
        warriors.Add(warrior);
        maximumHp += hpIncrease;
        maximumDamage += damageIncrease;
        RecountStats();
    }

    private void PlayShotAnimation(Player target)
    {
        foreach (People people in warriors)
        {
            people.PlayShotAnimation(target.transform);
        }

        foreach (Turret turret in turrets)
        {
            turret.ShootAnim(target.transform);
        }
    }
    
    private void PlayIdleAnimation()
    {
        if (isBoss)
        {
            warriors = GetComponentsInChildren<People>().ToList();
        }
        foreach (People people in warriors)
        {
            if (people == null)
                continue;

            people.IdleAnim();
        }
        
        foreach (Turret turret in turrets)
        {
            turret.IdleAnim();
        }
    }

    public void AttackPlayer(float dmg, Player target)
    {
        playerDmg = dmg;
        player = target;
        battle = true;
        
        PlayShotAnimation(target);
    }

    private void Dead()
    {
        InstantiateRewards();

        for(int i = 0; i < warriors.Count; i++)
        {
            warriors[i].PlayDyingAnimation();
            warriors.RemoveAt(i);
            damageClear -= damageIncrease;
            damageText.text = damageClear.ToString();
        }

        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
       
        Destroy(gameObject);
    }

    private void InstantiateRewards()
    {
        Vector3 pos = transform.position;
        if (Random.Range(0, 2) == 0)
            pos.x += Random.Range(5, 10);
        else
            pos.x -= Random.Range(5, 10);

        if (Random.Range(0, 2) == 0)
            pos.z += Random.Range(5, 10);
        else
            pos.z -= Random.Range(5, 10);
        foreach (AttachablePlatform t in platformsAdditive.Where(t => Random.Range(0f, 1f) > 0.7f))
        {
            Instantiate(t, pos, Quaternion.identity);
            if (Random.Range(0, 2) == 0)
                pos.x += Random.Range(5, 10);
            else
                pos.x -= Random.Range(5, 10);

            if (Random.Range(0, 2) == 0)
                pos.z += Random.Range(5, 10);
            else
                pos.z -= Random.Range(5, 10);
        }

        pos = transform.position;
        if (Random.Range(0, 2) == 0)
            pos.x += Random.Range(5, 10);
        else
            pos.x -= Random.Range(5, 10);

        if (Random.Range(0, 2) == 0)
            pos.z += Random.Range(5, 10);
        else
            pos.z -= Random.Range(5, 10);
        for (int i = 0; i < warriorsCount; i++)
        {
            if (Random.Range(0f, 1f) > 0.3f)
            {
                Instantiate(peopleAdditive[Random.Range(0, peopleAdditive.Count)], pos, Quaternion.identity);
                ;
                if (Random.Range(0, 2) == 0)
                    pos.x += Random.Range(3, 10);
                else
                    pos.x -= Random.Range(3, 10);

                if (Random.Range(0, 2) == 0)
                    pos.z += Random.Range(3, 10);
                else
                    pos.z -= Random.Range(3, 10);
            }
        }
    }

    private void CheckHP()
    {
        if (warriorsCount > 0)
        {
            if (maximumHp - currentHp <= (warriors.Count - 1) * hpIncrease )
            {
                if (warriors.Count > 0)
                {
                    People warrior = warriors[Random.Range(0, warriors.Count)];
                    warrior.PlayDyingAnimation();
                    warriors.Remove(warrior);
                    maximumDamage -= damageIncrease;
                }
            }
        }
        RecountStats();
    }

    public void GetDamage(float damage)
    {
        maximumHp -= damage;
        CheckHP();
        if (maximumHp <= 0)
        {
            battle = false;
            maximumHp = 0;
            isDead = true;
            Dead();
        }
    }


    public IEnumerable<GameObject> GetPlatforms()
    {
        return platforms.Select(x => x.gameObject).Concat(turrets.Select(x => x.transform.parent.gameObject));
    }
    
    private void OnDestroy()
    {
        Player.Died -= OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        battle = false;
        PlayIdleAnimation();
    }
}

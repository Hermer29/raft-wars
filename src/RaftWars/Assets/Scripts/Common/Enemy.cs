using System.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;
using DefaultNamespace;
using InputSystem;
using RaftWars.Infrastructure;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using Visual;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;


public class Enemy : FighterRaft, IPlatformsCarrier, ICanTakePeople
{
    [SerializeField] public List<Platform> platforms = new List<Platform>();
    [FormerlySerializedAs("maximumDamage")] [FormerlySerializedAs("fullDamage")] public float damage;
    [SerializeField] private float hpIncrease = PeopleConsts.StatsForPeople;
    [SerializeField] private float damageIncrease = PeopleConsts.StatsForPeople;
    [SerializeField] private bool _disableEdges;
    private List<People> warriors = new List<People>();
    private List<Turret> turrets = new List<Turret>();
    private int warriorsCount;
    private float hpClear = 0;
    private float damageClear = 0;
    private float turretDamage = 0;
    private float currentHp = 0;
    [FormerlySerializedAs("maximumHp")] [FormerlySerializedAs("fullHp")] public float hp;
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
    private EnemyHud _enemyHud;
    private const float SqrMagnitudeDistanceToReactOnPlayer = 10 * 10;
    private const int ExclusionSqrDistanceToPlayer = 300*2;
    private const int HpAdditive = PeopleConsts.StatsForPeople;
    private EdgesAndAngleWaves _edgesAndAngleWaves;
    private Coroutine _explosionsCoroutine;

    private int StatsSum => (int) (hp + damage);

    public Material Material
    {
        set
        {
            _material = value;
            SetColor(_material);
        }
    }

    public float Extents
    {
        get
        {
            if (_edgesAndAngleWaves == null)
                return 3;
            return _edgesAndAngleWaves.Bounds;
        }
    }

    private void Start()
    {
        _player = Game.PlayerService;
        _materialService = Game.MaterialsService;
        GenerateRandomColor(when: isBoss && _material == null);
        WarmupEdges();
        Player.Died += OnPlayerDied;
        if (!boss5Stage) return;
        WarmupPlatforms();
        AssignRelatedPeople();
        RecountStats();
    }

    private void TeleportFromOutOfBounds()
    {
        bool IsOutOfBounds()
        {
            return (transform.position.x > 45f || transform.position.x < -45f) && (transform.position.z > 45f ||
                transform.position.z < -45f);
        }

        if (!IsOutOfBounds()) return;
        Vector3 pointOnBound = GetNearestPointOnBound();
        Vector3 vectorToCenter = (Vector3.zero - pointOnBound).normalized;
        const int distanceFromBounds = 5;
        transform.position = vectorToCenter * distanceFromBounds + pointOnBound;
    }

    private void WarmupEdges()
    {
        if (_disableEdges)
            return;
        _edgesAndAngleWaves = gameObject.AddComponent<EdgesAndAngleWaves>();
        _edgesAndAngleWaves.Construct(this, _material);
        _edgesAndAngleWaves.CreateEdges();
        _edgesAndAngleWaves.CreateWaves();
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
            platforms[0].TryTakePeople(people.gameObject);
        }
    }
    
    public bool TryFindNotFullPlatform(out Platform platform)
    {
        platform = null;
        foreach(Platform plat in platforms)
        {
            if (plat.Capacity == 4 || plat.isTurret || plat.ishospital ||
                plat.isWind) continue;
            platform = plat;
            return true;
        }
        return false;
    }

    public override void AddPlatform(Platform platform)
    {
        platforms.Add(platform);
        _edgesAndAngleWaves.UpdateVisual(platform.gameObject);
    }

    public override void AddTurret(Turret turret)
    {
        turrets.Add(turret);
        hp += turret.healthIncrease;
        damage += turret.damageIncrease;
        RecountStats();
    }

    public override void AddFastTurret(Turret turret)
    {
        turrets.Add(turret);
        speed += turret.millSpeed;
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

    private void Update()
    {
        TeleportFromOutOfBounds();
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
        {
        }
        else if (_player.GameStarted == false)
            return;

        if (isDead)
            return;

        if (Bounds.IsInBounds(transform) == false)
        {
            _moveDirection = (Bounds.VectorToCenter(transform.position) +
                              new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5))).normalized;
        }
        else
        {
            if (_moveDirection == null)
            {
                MoveInRandomDirection();
            }

            float sqrMagnitudeToPlayer = (_player.Position - transform.position).sqrMagnitude;
            Vector3 escapeVector = -(_player.Position - transform.position).normalized;

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
            else if (_player.EnemyInExclusionZone == this)
            {
                _player.UnregisterEnemyInExclusionZone();
            }

            if (_player.IsDead)
            {
            }
            else if (sqrMagnitudeToPlayer < SqrMagnitudeDistanceToReactOnPlayer)
            {
                bool playerSuperior = _player.PlayerStatsSum >= StatsSum;
                _moveDirection = playerSuperior ? escapeVector : (_player.Position - transform.position).normalized;
            }
        }

        if (GameManager.instance.GamePaused)
            return;
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

    public void SpawnEnvironment(IEnumerable<Platform> platforms, People[] people, int hp, int damage, List<AttachablePlatform> platformsAdd, List<PeopleThatCanBeTaken> peopleAdd)
    {
        if (isBoss)
        {
            _enemyHud = GameFactory.CreateBossHud();
            _enemyHud.nickname.text = LanguageChanger.DescriptionProvider.Instance[LanguageChanger.TextName.Boss];
        }
        else
        {
            _enemyHud = GameFactory.CreateEnemyHud();
            var nick = _enemyHud.transform.Cast<Transform>()
                .First().Cast<Transform>()
                .First(x => x.name == "NicknameText")
                .GetComponent<TMP_Text>();
            var playerText = LanguageChanger.DescriptionProvider.Instance[LanguageChanger.TextName.Player];
            nick.text = playerText + Random.Range(1000, 10000);
        }

        _enemyHud.Target = transform;
        _enemyHud.transform.SetParent(Game.StatsCanvas.transform, worldPositionStays: false);
    
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
            var relatedTurret = plat.GetComponentInChildren<Turret>();
            this.platforms.Add(plat);
            if (plat.isTurret == false)
                continue;

            if (plat.ishospital)
            {
                currentHp += relatedTurret.healthIncrease;
                this.hp += relatedTurret.healthIncrease;
            }
            else if (plat.isTurret)
            {
                turrets.Add(relatedTurret);
                turretDamage += relatedTurret.damageIncrease;
                this.damage += relatedTurret.damageIncrease;
                currentHp += relatedTurret.healthIncrease;
                this.hp += relatedTurret.healthIncrease;
                relatedTurret.DrawInMyColor(_material);
            }   
        }

        foreach (People man in people)
        {
            this.platforms[Random.Range(0, this.platforms.Count)]
                .TryTakePeople(man.gameObject);
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

            int otherEnemies = LayerMask.GetMask("Enemy");
            var outCols = Physics.OverlapSphere(startPoint, 1, otherEnemies);
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
        if(_enemyHud == null)
        {
            if(_enemyHud == null)
            {
                _enemyHud = GameFactory.CreateBossHud();
                _enemyHud.transform.SetParent(Game.StatsCanvas.transform, worldPositionStays: false);
            }
        }

        _enemyHud.hpText.text = Mathf.RoundToInt(hp).ToString();
        _enemyHud.damageText.text = Mathf.RoundToInt(damage).ToString();
    }
    
    private Vector3 GetScaledRandomPointAmongAllPlatforms()
    {
        Platform randomPlatform = platforms.ElementAt(Random.Range(0, platforms.Count));
        return randomPlatform.GetRandomPoint();
    }

    public void AddPeople(People warrior)
    {
        warriors.Add(warrior);
        hp += hpIncrease;
        damage += damageIncrease;
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

    public void Dead()
    {
        hp = 0;
        isDead = true;
        InstantiateRewards();
        _enemyHud.Target = null;

        for(int i = 0; i < warriors.Count; i++)
        {
            warriors[i].PlayDyingAnimation();
            warriors.RemoveAt(i);
            damageClear -= damageIncrease;
            _enemyHud.damageText.text = damageClear.ToString();
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

    private void MakeRandomPeopleDie()
    {
        if(warriors.Count == 1)
            return;
        People warrior = warriors[Random.Range(0, warriors.Count)];
        warrior.MakeGrey();
        warrior.PlayDyingAnimation();
        warriors.Remove(warrior);
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

    public override EnemyHud GetHud()
    {
        return _enemyHud;
    }

    public override void DealDamage(int amount = 1)
    {
        bool IsRandomPeopleMustDie()
        {
            return (int) hp % hpIncrease == 0;
        }
        
        if (IsRandomPeopleMustDie())
        {
            MakeRandomPeopleDie();
            damage -= damageIncrease;
            if (damage <= 0)
                damage = 0;
        }
        hp -= amount;
        if (hp <= 0)
            hp = 0;
        
        RecountStats();
    }

    public override void Die()
    {
        Dead();
        if(_explosionsCoroutine != null)
            StopCoroutine(_explosionsCoroutine);
    }

    public override void StopFight()
    {
        if(_explosionsCoroutine != null)
            StopCoroutine(_explosionsCoroutine);
        OnPlayerDied();
    }

    public void StartFight()
    {
        battle = true;
        PlayShotAnimation(_player.PlayerInstance);
        _explosionsCoroutine = StartCoroutine(CreateExplosions(CancellationToken.None));
    }
    
    private IEnumerator CreateExplosions(CancellationToken token, Enemy enemy = null)
    {
        const float explosionFrequency = 1f;
        
        while (true)
        {
            if (token.IsCancellationRequested)
                yield break;
            CreateExplosions();
            yield return new WaitForSeconds(explosionFrequency);
        }
    }
    
    private void CreateExplosions()
    {
        Explosion explosion = GameFactory.CreateExplosion();
        explosion.transform.position = GetScaledRandomPointAmongAllPlatforms();
    }

    public bool TryTakePeople(GameObject warrior)
    {
        throw new NotImplementedException("Should not be called, cause interface is just a marker");
    }

    public IEnumerable<Vector3> GetPlatformPoints()
    {
        return _edgesAndAngleWaves.GetAllBorderPoints();
    }
}

using System.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;
using DefaultNamespace;
using Infrastructure;
using InputSystem;
using LanguageChanger;
using RaftWars.Infrastructure;
using SpecialPlatforms;
using SpecialPlatforms.Concrete;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Visual;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using ValueType = SpecialPlatforms.ValueType;
using Vector3 = UnityEngine.Vector3;

public class Enemy : FighterRaft, IPlatformsCarrier, ICanTakePeople
{
    [SerializeField] public List<Platform> platforms = new List<Platform>();
    [FormerlySerializedAs("maximumDamage")] [FormerlySerializedAs("fullDamage")] public float damage;
    [SerializeField] private float hpIncrease = PeopleConsts.StatsForPeople;
    [SerializeField] private float damageIncrease = PeopleConsts.StatsForPeople;
    [SerializeField] private bool _disableEdges;
    public List<People> warriors = new List<People>();
    private List<Turret> turrets = new List<Turret>();
    private int warriorsCount;
    private float hpClear = 0;
    private float damageClear = 0;
    private float _relativeHp;
    private float _relativeDamage;
    private float _actualHp;
    private float _actualDamage;
    [FormerlySerializedAs("maximumHp")] [FormerlySerializedAs("fullHp")] public float hp;
    public bool battle;
    private float relativeSpeed;
    private Player player;
    private float timer = 0;
    public bool isDead;
    public bool isBoss = false;
    private List<PeopleThatCanBeTaken> peopleAdditive = new List<PeopleThatCanBeTaken>();
    private List<AttachablePlatform> platformsAdditive = new List<AttachablePlatform>();
    private Vector3 prevSpawnPoint;
    private PlayerService _player;
    public bool boss5Stage = true;
    public bool hasShield;
    public List<GameObject> enemiesToKill;
    public GameObject shieldToOff;
    private bool _fleeingPlayer;
    public Material _material;
    private MaterialsService _materialService;
    
    private const float RunningViewportBounds = .05f;
    private const int HpAdditive = PeopleConsts.StatsForPeople;

    private EnemyHud _statsHud;
    private EdgesAndAngleWaves _edgesAndAngleWaves;
    private Coroutine _explosionsCoroutine;
    private IEnumerable<SpecialPlatform> _platformsData;

    public int StatsSum => Health + Damage;

    public event Action Died;

    public Material Material
    {
        set
        {
            _material = value;
            SetColor(_material);
        }
    }

    public override int PlatformsCount => platforms.Count;
    public override int Damage => (int)(damage + Mathf.Abs(damage * _relativeDamage));
    public override int Health => (int)(hp + Mathf.Abs(hp * _relativeHp));
    public override float MoveSpeed => (int)(Speed + Speed * relativeSpeed);

    public float Speed { private set; get; } = 5 - 5/4;
    public bool InBattle => battle;
    public bool IsDead => isDead;

    public new void Construct(IEnumerable<SpecialPlatform> platformsBalanceData)
    {
        base.Construct(platformsBalanceData, useDefaultBalanceValues: true);
    }
    
    private void Start()
    {
        _player = Game.PlayerService;
        _materialService = Game.MaterialsService;
        transform.AddComponent<DrivingEnemy>()
            .Construct(this, _player, Game.FightService);
        GenerateRandomColor(when: isBoss && _material == null);
        Player.Died += OnPlayerDied;
        if (!_disableEdges && _edgesAndAngleWaves == null)
        {
            CreateEdges();
        }
        if (!boss5Stage) return;
        InitializeTurretsFor5StageBoss();
        AssignRelatedPeopleFor5StageBoss();
        RecountStats();
    }

    private void CreateEdges()
    {
        if (_disableEdges)
            return;
        
        _edgesAndAngleWaves = gameObject.AddComponent<EdgesAndAngleWaves>()
            .Construct(this, _material);
        _edgesAndAngleWaves.CreateEdges();
        _edgesAndAngleWaves.CreateWaves();
    }

    private void GenerateRandomColor(bool when)
    {
        if (!when)
            return;

        _material = boss5Stage ? _materialService.GetMaterialForBoss5Stage() 
            : _materialService.GetRandom();
        SetColor(_material);
    }

    private void SetColor(Material material)
    {
        foreach (People people in warriors)
        {
            people.Material = material;
        }
    }

    private void AssignRelatedPeopleFor5StageBoss()
    {
        foreach (People people in GetComponentsInChildren<People>(includeInactive: true))
        {
            platforms[0].TryTakePeople(people.gameObject);
            people.Material = _material;
        }
    }
    
    public bool TryFindNotFullPlatform(out Platform sought)
    {
        sought = null;
        foreach(Platform platform in platforms)
        {
            if (platform.Capacity == 4 || platform.isTurret || platform.ishospital ||
                platform.isWind) continue;
            sought = platform;
            return true;
        }
        return false;
    }

    protected override void AddPlatform(Platform platform)
    {
        var turret = platform.GetComponentInChildren<Turret>();
        if(turret != null)
            turrets.Add(turret);
        platforms.Add(platform);
        _edgesAndAngleWaves?.UpdateVisual();
    }

    public override bool TryGetNotFullPlatform(out Platform platform)
    {
        return TryFindNotFullPlatform(out platform);
    }

    protected override void AddDamageForPlatformType(Type data)
    {
        var platform = FindPlatformDataWithConcreteType<IDamageAmplifying>(data);
        if (platform.ValueType == ValueType.Absolute)
        {
            damage += platform.BaseDamage;
        }
        else if (platform.ValueType == ValueType.Relative)
        {
            _relativeDamage += platform.BaseDamage;
        }
        RecountStats();
    }

    protected override void AddSpeedForPlatformType(Type type)
    {
        var platformData = FindPlatformDataWithConcreteType<ISpeedIncreasing>(type);
        if (platformData.ValueType == ValueType.Absolute)
        {
            Speed += platformData.DefaultSpeedBonus;
        }
        else if (platformData.ValueType == ValueType.Relative)
        {
            relativeSpeed += platformData.DefaultSpeedBonus;
        }
    }

    private void InitializeTurretsFor5StageBoss()
    {
        foreach (Platform platform in platforms)
        {
            platform.isEnemy = true;
            platform.gameObject.layer = LayerMask.NameToLayer("Enemy");
            if (!platform.isTurret) continue;
            turrets.Add(platform.GetComponentInChildren<Turret>());
        }
    }

    private void Update()
    {
        if (hasShield)
        {
            for(var i = 0; i < enemiesToKill.Count; i++)
            {
                if (enemiesToKill[i] == null)
                    enemiesToKill.RemoveAt(i);
                if (enemiesToKill.Count != 0) continue;
                shieldToOff.SetActive(false);
                hasShield = false;
            }
        }

        if (player != null && !player.isDead) return;
        battle = false;
        timer = 0;
    }

    public override Platform GetAnotherPlatform()
    {
        return platforms[Random.Range(0, platforms.Count)];
    }

    public void SpawnEnvironment(IEnumerable<Platform> prefabPlatforms, People[] people, int hp, int damage, List<AttachablePlatform> platformsAdd, List<PeopleThatCanBeTaken> peopleAdd)
    {
        CreateStatsHud();

        if (boss5Stage)
        {
            _materialService = Game.MaterialsService;
            this.platforms[0].Material = _materialService.GetMaterialForBoss5Stage();
            foreach (People man in people)
            {
                this.platforms[0].TryTakePeople(man.gameObject);
            }
            return;
        }
        
        CreateEdges();
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
        
        foreach (Platform prefabPlatform in prefabPlatforms)
        {
            startPoint = ThinkOutSpawnPosition(startPoint);
            Platform instantiated = Instantiate(prefabPlatform, startPoint, Quaternion.identity, transform);
            instantiated.Material = _material;
            instantiated.isEnemy = true;
            instantiated.gameObject.layer = LayerMask.NameToLayer("Enemy");
            AddAbstractPlatform(instantiated, _material);
        }

        foreach (People man in people)
        {
            platforms[Random.Range(0, this.platforms.Count)]
                .TryTakePeople(man.gameObject);
        }

        RecountStats();
    }

    private void CreateStatsHud()
    {
        _statsHud = GameFactory.CreateStatsHud();
        _statsHud.Target = transform;
        _statsHud.transform.SetParent(Game.StatsCanvas.transform, worldPositionStays: false);

        if (isBoss)
        {
            _statsHud.nickname.text = LocalizationService.Instance[TextName.Boss];
            return;
        }
        var nick = _statsHud.transform.Cast<Transform>()
            .First().Cast<Transform>()
            .First(x => x.name == "NicknameText")
            .GetComponent<TMP_Text>();
        string playerText = LocalizationService.Instance[TextName.Player];
        nick.text = playerText + Random.Range(1000, 10000);
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
        if(_statsHud == null)
        {
            // Мы в этот код попадаем только если враг не создавался генератором, 
            // к примеру придаток босса с пятого этапа попадает сюда
            // потому шо придатки босса содержатся в префабе основного босса
            _statsHud = GameFactory.CreateStatsHud();
            _statsHud.transform.SetParent(Game.StatsCanvas.transform, worldPositionStays: false);
            if(_statsHud.Target == null)
            {
                _statsHud.Target = transform;
            }
            _statsHud.nickname.text = "";
        }

        _statsHud.hpText.text = Health.ToString();
        _statsHud.damageText.text = Damage.ToString();
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

        if(boss5Stage)
        {
            foreach(var people in GetComponentsInChildren<People>())
            {
                people.PlayShotAnimation(target.transform);
            }
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

    public void Dead()
    {
        hp = 0;
        isDead = true;
        InstantiateRewards();
        _statsHud.Target = null;

        for(int i = 0; i < warriors.Count; i++)
        {
            warriors[i].PlayDyingAnimation();
            warriors.RemoveAt(i);
            damageClear -= damageIncrease;
            _statsHud.damageText.text = damageClear.ToString();
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
       
        Died?.Invoke();
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
        return platforms.Select(x => x.gameObject).ToArray();
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

    protected override void AddHealthForPlatformType(Type data)
    {
        var relatedPlatform = FindPlatformDataWithConcreteType<IHealthIncreasing>(data);
        if (relatedPlatform.ValueType == ValueType.Absolute)
        {
            hp += relatedPlatform.DefaultHealthGain;
        }
        else if (relatedPlatform.ValueType == ValueType.Relative)
        {
            _relativeHp += relatedPlatform.DefaultHealthGain;
        } 
        RecountStats();
    }

    protected override void AddMagnetWithPlatformType(Type data, Turret turret)
    {
        
    }

    public override EnemyHud GetHud()
    {
        return _statsHud;
    }

    public override void DealDamage(int amount = 1)
    {
        bool IsRandomPeopleMustDie()
        {
            return (int) hp % hpIncrease == 0;
        }
        
        float changed = hp - amount;
        float percent = changed / hp;
        float newDamage = damage * percent;
        damage = newDamage;

        if (IsRandomPeopleMustDie())
        {
            MakeRandomPeopleDie();
        }
        hp -= amount;
        if (Mathf.Floor(hp) <= 0)
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

    public bool TryTakePeople(GameObject warriorPrefab, Vector3? specifiedSpawnPoint)
    {
        throw new NotImplementedException("Should not be called, cause interface is just a marker");
    }
}

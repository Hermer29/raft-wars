using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cinemachine;
using Common;
using DefaultNamespace;
using DG.Tweening;
using Infrastructure;
using InputSystem;
using Interface;
using RaftWars.Infrastructure;
using Skins;
using Skins.Hats;
using Skins.Platforms;
using SpecialPlatforms;
using SpecialPlatforms.Concrete;
using Units.Enemies;
using UnityEngine;
using UnityEngine.Serialization;
using Visual;
using Random = UnityEngine.Random;
using ValueType = SpecialPlatforms.ValueType;

public class Player : FighterRaft, IPlatformsCarrier, ICanTakeBarrel, ICanTakeCoins, ICanTakePeople, ITargetable
{
    [SerializeField] public List<People> warriors;
    [SerializeField] private List<Platform> platforms;
    [SerializeField] private List<Turret> turrets = new List<Turret>();
    [FormerlySerializedAs("maximumHp")] [FormerlySerializedAs("fullHp")] public int hp;
    [FormerlySerializedAs("maximumDamage")] [FormerlySerializedAs("fullDamage")] public int damage;
    [FormerlySerializedAs("hpIncrease")] [SerializeField] private int hpIncomeForPeople = PeopleConsts.StatsForPeople;
    [FormerlySerializedAs("damageIncrease")] [SerializeField] private int damageIncomeForPeople = PeopleConsts.StatsForPeople;
    [SerializeField] public float speed;
    public CinemachineTargetGroup CameraGroup;
    private bool _movingStarted = false;

    public override int PlatformsCount => platforms.Count;
    public override int Damage => (int) (damage + damage * relativeDamage);
    public override int Health => (int) (hp + hp * relativeHp);
    public override float MoveSpeed => (int) (speed + speed * relativeSpeed);
    
    private float relativeHp;
    private float relativeDamage;
    private float relativeSpeed;
    private float _actualHp;
    private float _actualDamage;
    public bool battle;
    public bool isDead;
    public bool canPlay;
    public int coins;
    public int gems;
    public int warriorsCount = 2;
    public CinemachineCameraOffset _camera;
    private const int PushForceBeforeFightFromEnemy = 2;

    private Enemy enemyForBattle;
    private InputService _input;
    public static Player instance;
    private Rigidbody rb;
    private Material _material;
    private bool idleBehaviour;
    private EdgesAndAngleWaves edgesAndAngleWaves;
    private Hud _hud;
    private HatSkin _hat;
    private EnemyHud _enemyHud;
    private Coroutine _explosionsCoroutine;
    public static event Action Died;
    public Transform center;
    private int _healthBeforeFight;
    private float _lostPercent;
    private int _damageBeforeFight;
    private float _lostPercentForPeople;
    private float _stepForPeoplrCapacityBeforeBattle;

    public Vector3 MoveDirectionXZ => new(_input.Horizontal, 0, _input.Vertical);

    public void Construct(IEnumerable<SpecialPlatform> specialPlatforms)
    {
        base.Construct(
            specialPlatforms, 
            useDefaultBalanceValues: false);
        edgesAndAngleWaves = gameObject.AddComponent<EdgesAndAngleWaves>();
        edgesAndAngleWaves.Construct(this);
        platforms[0].Capacity = 2;
    }
    
    private void Start()
    {
        _enemyHud = GameFactory.CreateStatsHud();
        _enemyHud.transform.SetParent(Game.StatsCanvas.transform, worldPositionStays: false);
        _enemyHud.Target = center;
        _enemyHud.CannotBeReplaced = true;
        _enemyHud.WorksInFixedUpdate = true;
        _enemyHud.NotParticipateInPrioritization = true;
        _input = Game.InputService;
        _hud = Game.Hud;
        
        hp = PeopleConsts.StatsForPeople * warriorsCount;
        damage = PeopleConsts.StatsForPeople * warriorsCount;
        MakePeopleRunAndColorize(_material);
        if (instance == null)
            instance = this;
        else
        {
            Destroy(instance);
            instance = this;
        }

        rb = GetComponent<Rigidbody>();
        RecountStats();
        CreateEdges();
        TryGenerateNickname();
    }

    private void MakePeopleRunAndColorize(Material material)
    {
        var onlyPlatform = GetComponentInChildren<Platform>();
        onlyPlatform.Material = material;
        foreach (People componentsInChild in GetComponentsInChildren<People>())
        {
            componentsInChild.SetRelatedPlatform(onlyPlatform);
        }
    }

    private void CreateEdges()
    {
        edgesAndAngleWaves.CreateEdges();
        edgesAndAngleWaves.CreateWaves();
    }

    private void TryGenerateNickname()
    {
        string nick = LanguageChanger.LocalizationService.Instance[LanguageChanger.TextName.Player];
        _enemyHud.nickname.text = nick;
    }

    private void Update()
    {
        if (!battle) return;
        if (!enemyForBattle.isDead && enemyForBattle != null) return;
        if (idleBehaviour)
            return;
        OnBattleEnded();
    }

    private void OnBattleEnded()
    {
        battle = false;
        idleBehaviour = true;
        PutInIdleAnimation();
        _lostPercent = 0;
        _lostPercentForPeople = 0;
    }

    private void PutInIdleAnimation()
    {
        foreach (People people in warriors)
        {
            people.IdleAnim();
        }

        foreach (Turret turret in turrets)
        {
            turret.IdleAnim();
        }
        if(_explosionsCoroutine != null)
            StopCoroutine(_explosionsCoroutine);
    }

    public void AddGems(int gems)
    {
        this.gems += gems;
        _hud.ShowGems(this.gems);
    }

    public void AddPeople(People warrior)
    {
        warriors.Add(warrior);
        hp += PeopleConsts.StatsForPeople;
        damage += PeopleConsts.StatsForPeople;
        warrior.Material = _material;
        RecountStats();
        warrior.ApplyHat(_hat);
    }
    
    public override EnemyHud GetHud()
    {
        return _enemyHud;
    }

    public override void DealDamage(int amount = 1)
    {
        var changedHp = hp - amount;

        var damagePercent = (float) changedHp / _healthBeforeFight;
        if(Game.FightService.FightStarted == false)
        {
            damage -= amount;
        }
        else
            damage = (int) Mathf.Ceil(_damageBeforeFight * damagePercent);

        bool willPeopleDie = changedHp < _healthBeforeFight - _lostPercentForPeople * _healthBeforeFight;

        if(willPeopleDie)
        {
            MakeRandomPeopleDie();
            _lostPercentForPeople += _stepForPeoplrCapacityBeforeBattle;
        }

        hp -= amount;
        if (Game.FightService.FightStarted == false && hp <= 0)
        {
            Die();
        }
        RecountStats();
    }

    private void RecountStats()
    {
        _enemyHud.hpText.text = Mathf.RoundToInt(Mathf.Clamp(Health, 0, 99999)).ToString();
        _enemyHud.damageText.text = Mathf.RoundToInt(Mathf.Clamp(Damage, 0, 99999)).ToString();
    }

    private void MakeRandomPeopleDie()
    {
        if (warriors.Count == 1)
            return;

        People warrior = warriors[Random.Range(0, warriors.Count)];
        warrior.PlayDyingAnimation(!TryThrowPeopleInWater(warrior));
        warrior.MakeGrey();
        warriors.Remove(warrior);
    }

    private bool TryThrowPeopleInWater(People people)
    {
        bool randomBool = Random.Range(0, 2) == 1;
        if (randomBool)
        {
            people.ThrowAway();
        }
        return randomBool;
    }

    private void FixedUpdate()
    {
        if (_input == null)
            return;

        if (isDead)
            battle = false;

        if (!canPlay)
        {
            rb.velocity = Vector3.zero;
            Game.AudioService.StopPlayingSwimmingSound();
            return;
        }
        
        if (!battle)
        {
            rb.velocity = MoveDirectionXZ * speed;
            HandleMovementEvents(MoveDirectionXZ);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void HandleMovementEvents(Vector3 moveDirectionXz)
    {
        switch (_movingStarted)
        {
            case false when moveDirectionXz.sqrMagnitude != 0:
                _movingStarted = true;
                Game.AudioService.PlaySwimmingSound();
                break;
            case true when moveDirectionXz.sqrMagnitude == 0:
                _movingStarted = false;
                Game.AudioService.StopPlayingSwimmingSound();
                break;
        }
    }
    

    public void StartBattle(Enemy enemy)
    {
        Game.FightService.FightBeginningCollisionDetected(enemy);
        
        StartExplosions(CancellationToken.None);
        if (battle) return;

        idleBehaviour = false;
        enemyForBattle = enemy;
        battle = true;
        MakeWarriorsShot(enemy);
        MakeTurretsShot(enemy);
        PushPlayerOutOfEnemy(enemy);
        _healthBeforeFight = hp;
        _damageBeforeFight = damage;
        _lostPercent = 0;
        _lostPercentForPeople = 0;
        _stepForPeoplrCapacityBeforeBattle = 1f / warriors.Count;
    }
    
    private void PushPlayerOutOfEnemy(Enemy enemy)
    {
        rb.AddForce((transform.position - enemy.transform.position).normalized * PushForceBeforeFightFromEnemy, ForceMode.Impulse);
    }

    private void MakeTurretsShot(Enemy enemy)
    {
        foreach (Turret turret in turrets)
        {
            turret.ShootAnim(enemy.transform);
        }
    }

    private void MakeWarriorsShot(Enemy enemy)
    {
        foreach (People warrior in warriors)
        {
            warrior.PlayShotAnimation(enemy);
        }
    }

    private void Dead()
    {
        isDead = true;
        canPlay = false;
        battle = false;
        PutTurretsInIdleAnimation();
        foreach (People warrior in warriors)
        {
            warrior.PlayDyingAnimation();
        }
        GameManager.instance.PlayerLost();
        Died?.Invoke();
        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
        RepaintWith(RaftWars.Infrastructure.AssetManagement.AssetLoader.LoadPlayerDeathMaterial());

        _enemyHud.Target = null;
        if(enemyForBattle != null)
        {
            foreach(Platform platform in enemyForBattle.platforms)
            {   
                CameraGroup.RemoveMember(platform.transform);
            }
        }
        if (_explosionsCoroutine != null)
            StopCoroutine(_explosionsCoroutine);
        StartExplosions(new CancellationTokenSource().Token);
    
        var virtualCamera = _camera.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.m_Follow = null;
        virtualCamera.m_LookAt = CameraGroup.transform;
        transform.DOMoveY(-999, .65f).SetSpeedBased(true);
    }

    private void StartExplosions(CancellationToken token)
    {
        if(_explosionsCoroutine != null)
        {
            StopCoroutine(_explosionsCoroutine);
        }
        _explosionsCoroutine = StartCoroutine(CreateExplosions(token));
    }

    private IEnumerator CreateExplosions(CancellationToken token)
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

    private Vector3 GetScaledRandomPointAmongAllPlatforms()
    {
        Platform randomPlatform = platforms.ElementAt(Random.Range(0, platforms.Count));
        return randomPlatform.GetRandomPoint();
    }

    private void PutTurretsInIdleAnimation()
    {
        foreach (Turret turret in turrets)
        {
            turret.IdleAnim();
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

    public override Platform GetAnotherPlatform()
    {
        return platforms[Random.Range(0, platforms.Count)];
    }

    protected override void AddHealthForPlatformType(Type data)
    {
        var statsHolder = FindPlatformDataWithConcreteType<IHealthIncreasing>(data);
        if (statsHolder.ValueType == ValueType.Absolute)
        {
            hp += (int) statsHolder.HealthValue;
        }
        else if (statsHolder.ValueType == ValueType.Relative)
        {
            relativeHp += statsHolder.HealthValue;
        }
        RecountStats();
    }

    protected override void AddDamageForPlatformType(Type data)
    {
        var statsHolder = FindPlatformDataWithConcreteType<IDamageAmplifyer>(data);
        switch (statsHolder.ValueType)
        {
            case ValueType.Absolute:
                damage += (int) statsHolder.DamageValue;
                break;
            case ValueType.Relative:
                relativeDamage += statsHolder.DamageValue;
                break;
        }
        RecountStats();
    }

    protected override void AddSpeedForPlatformType(Type data)
    {
        var statsHolder = FindPlatformDataWithConcreteType<ISpeedIncreasing>(data);
        switch (statsHolder.ValueType)
        {
            case ValueType.Absolute:
                speed += statsHolder.SpeedBonus;
                break;
            case ValueType.Relative:
                relativeSpeed += statsHolder.SpeedBonus;
                break;
        }
        RecountStats();
    }

    protected override void AddMagnetWithPlatformType(Type data, Turret turret)
    {
        var magnet = FindPlatformDataWithConcreteType<Magnet>(data);
        (turret as IMagnetTurret).ModifyPickingSpace(magnet.CollectingRadius);
    }

    protected override void AddPlatform(Platform platform)
    {
        void AddPlatformToCameraTargetGroup()
        {
            CameraGroup.AddMember(platform.transform, 1, 7);
        }

        platforms.Add(platform);
        AddPlatformToCameraTargetGroup();
        edgesAndAngleWaves.UpdateVisual();
        _camera.m_Offset.z = edgesAndAngleWaves.Bounds * -1;
        speed += Game.FeatureFlags.PlayerSpeedIncreasingPerPlatform;
    }

    public override bool TryGetNotFullPlatform(out Platform platform)
    {
        return TryFindNotFullPlatform(out platform);
    }

    public void AmplifyDamage(float percent)
    {
        //usually percent = 0.2
        relativeDamage += percent;
        RecountStats();
    }

    public void IncreaseHealth(float bonus)
    {
        relativeHp += bonus;
        RecountStats();
    }

    public IEnumerable<GameObject> GetPlatforms()
    {
        return platforms.Select(x => x.gameObject);
    }

    public void RepaintWith(PlayerColors colors)
    {
        RepaintWith(colors.Color);
    }

    public void RepaintWith(Material material)
    {
        _material = material;
        edgesAndAngleWaves?.ChangeMaterial(material);
        RepaintPlatforms(material);
        RepaintPeople(material);
    }

    public void ApplyHat(HatSkin hat)
    {
        _hat = hat;
        foreach (People warrior in warriors)
        {
            warrior.ApplyHat(hat);
        }
    }

    public void ApplyPlatformSkin(PlatformSkin skin)
    {
        switch (skin.HasEdges)
        {
            case false when edgesAndAngleWaves.EdgesDisabled == false:
                edgesAndAngleWaves.DisableEdges();
                break;
            case true when edgesAndAngleWaves.EdgesDisabled:
                edgesAndAngleWaves.EnableEdges();
                break;
        }

        foreach (Platform platform in platforms)
        {
            platform.ApplySkin(skin);
        }
    }

    private void RepaintPeople(Material material)
    {
        foreach (People warrior in warriors)
        {
            warrior.Material = material;
        }
    }

    private void RepaintPlatforms(Material material)
    {
        foreach (Platform platform in platforms)
        {
            platform.Material = material;
        }
    }

    public override void Die()
    {
        Dead();
    }

    public override void StopFight()
    {
        if (_explosionsCoroutine != null)
            StopCoroutine(_explosionsCoroutine);
        OnBattleEnded();
    }

    bool ICanTakeBarrel.TryTakeBarrel(int damage)
    {
        if(Health < 50)
            return true;
        DealDamage(damage);
        return true;
    }

    bool ICanTakeCoins.TryTakeCoins(int coins)
    {
        Game.MoneyService.AddCoins(coins);
        return true;
    }

    bool ICanTakePeople.TryTakePeople(GameObject warriorPrefab, Vector3? specifiedSpawnPoint)
    {
        throw new InvalidOperationException("Should not be called, cause interface is just a marker");
    }

    public void IncreaseStats(int amount)
    {
        damage += amount;
        hp += amount;
        RecountStats();
    }

    public void Amplify(int amount)
    {
        IncreaseStats(amount);
    }

    public bool TryTakePeople(GameObject warriorPrefab, Vector3? specifiedSpawnPoint = null)
    {
        throw new NotImplementedException();
    }

    public Vector3 GetRandomTarget()
    {
        return platforms.Random().GetRandomPoint();
    }
}
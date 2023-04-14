using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Agava.YandexGames;
using Cinemachine;
using Common;
using DefaultNamespace;
using DG.Tweening;
using InputSystem;
using Interface;
using RaftWars.Infrastructure;
using Skins;
using Skins.Hats;
using Skins.Platforms;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;
using Visual;
using Random = UnityEngine.Random;

public class Player : FighterRaft, IPlatformsCarrier, ICanTakeBarrel, ICanTakeCoins, ICanTakePeople
{
    [SerializeField] private List<People> warriors;
    [SerializeField] private List<Platform> platforms;
    [SerializeField] private List<Turret> turrets = new List<Turret>();
    [SerializeField] private TextMeshPro hpText;
    [SerializeField] private TextMeshPro damageText;
    [SerializeField] private TextMeshPro nickname;
    [SerializeField] private Text coinsText, gemsText;
    [FormerlySerializedAs("maximumHp")] [FormerlySerializedAs("fullHp")] public int hp;
    [FormerlySerializedAs("maximumDamage")] [FormerlySerializedAs("fullDamage")] public int damage;
    [FormerlySerializedAs("hpIncrease")] [SerializeField] private int hpIncomeForPeople = PeopleConsts.StatsForPeople;
    [FormerlySerializedAs("damageIncrease")] [SerializeField] private int damageIncomeForPeople = PeopleConsts.StatsForPeople;
    [SerializeField] public float speed;
    public CinemachineTargetGroup CameraGroup;
    [SerializeField] private GameObject[] _indicators;

    private int platformHp;
    private float hpAdditive;
    private float damageAdditive;
    public bool battle;
    public bool isDead;
    public int platformCount = 1;
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
    private MaterialsService _materialsService;
    private Material _material;
    private bool idleBehaviour;
    private EdgesAndAngleWaves edgesAndAngleWaves;
    private Hud _hud;
    private HatSkin _hat;
    private EnemyHud _enemyHud;
    private Coroutine _explosionsCoroutine;
    public static event Action Died;
    public Transform center;

    public Vector3 MoveDirectionXZ => new(_input.Horizontal, 0, _input.Vertical);
    public float Bounds => edgesAndAngleWaves.Bounds;

    public void Construct()
    {
        edgesAndAngleWaves = gameObject.AddComponent<EdgesAndAngleWaves>();
        edgesAndAngleWaves.Construct(this);
        platforms[0].Capacity = 2;
    }
    
    private void Start()
    {
        _enemyHud = GameFactory.CreateEnemyHud();
        _enemyHud.transform.SetParent(Game.StatsCanvas.transform, worldPositionStays: false);
        _enemyHud.Target = center;
        _enemyHud.CannotBeReplaced = true;
        _enemyHud.WorksInFixedUpdate = true;
        _input = Game.InputService;
        _materialsService = Game.MaterialsService;
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
        string nick = LanguageChanger.DescriptionProvider.Instance[LanguageChanger.TextName.Player];
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

    public void AddCoins(int coins)
    {
        this.coins += coins;
        _hud.ShowCoins(this.coins);
    }

    public void AddGems(int gems)
    {
        this.gems += gems;
        _hud.ShowGems(this.gems);
    }

    public void AddPeople(People warrior)
    {
        warriors.Add(warrior);
        hp += 6;
        damage += 6;
        warrior.Material = _material;
        RecountStats();
        warrior.ApplyHat(_hat);
    }

    public void AddHealTurret(int healthIncrease)
    {
        platformHp += healthIncrease;
        hp += (int) (healthIncrease * (1 + hpAdditive));
        RecountStats();
    }

    public override EnemyHud GetHud()
    {
        return _enemyHud;
    }

    public override void DealDamage(int amount = 1)
    {
        const int damageAdditive = 6;

        bool IsRandomPeopleMustDie()
        {
            return hp % damageAdditive == 0;
        }
        if (IsRandomPeopleMustDie())
        {
            MakeRandomPeopleDie();
            damage -= damageAdditive;
            if (damage <= 0)
                damage = 0;
        }
        hp -= amount;
        if (Game.FightService.FightStarted == false && hp <= 0)
        {
            Die();
        }
        RecountStats();
    }

    public override void AddTurret(Turret tur)
    {
        turrets.Add(tur);
        platformHp += tur.healthIncrease;
        hp += (int) (tur.healthIncrease * (1 + hpAdditive));
        damage += (int) (tur.damageIncrease * (1 + damageAdditive));
        RecountStats();
    }
    
    public override void AddFastTurret(Turret tur)
    {
        this.speed += speed;
    }

    private void RecountStats()
    {
        _enemyHud.hpText.text = Mathf.RoundToInt(Mathf.Clamp(hp, 0, 99999)).ToString();
        _enemyHud.damageText.text = Mathf.RoundToInt(Mathf.Clamp(damage, 0, 99999)).ToString();
        warriorsCount = warriors.Count;
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
            return;
        }
        
        if (!battle)
        {
            rb.velocity = MoveDirectionXZ * speed;
        }
        else
        {
            rb.velocity = Vector3.zero;
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
            warrior.PlayShotAnimation(enemy.transform);
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

    public Platform GetPlatformWithoutTurret()
    {
        Platform _platform;
        while (true)
        {
            _platform = platforms[Random.Range(0, platformCount)];
            if (!_platform.isTurret)
            {
                break;
            }
        }
        return _platform;
    }

    public Platform GetAnotherPlatform()
    {
        return platforms[Random.Range(0, platformCount)];
    }

    public override void AddPlatform(Platform platform)
    {
        void AddPlatformToCameraTargetGroup()
        {
            CameraGroup.AddMember(platform.transform, 1, 7);
        }

        platformCount++;
        platforms.Add(platform);
        AddPlatformToCameraTargetGroup();
        edgesAndAngleWaves.UpdateVisual(platform.gameObject);
        _camera.m_Offset.z = edgesAndAngleWaves.Bounds * -1;
        speed += .5f;
    }

    public void AmplifyDamage(float percent)
    {
        damage += (int) (damage * (percent - damageAdditive));
        damageAdditive = percent;
    }

    public void IncreaseHealth(float bonus)
    {
        hp += (int) (hp * (bonus - hpAdditive));
        hpAdditive = (int) bonus;
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

    public bool TryTakeBarrel(int damage)
    {
        DealDamage(damage);
        return true;
    }

    public bool TryTakeCoins(int coins)
    {
        Game.MoneyService.AddCoins(coins);
        return true;
    }

    public bool TryTakePeople(GameObject warrior)
    {
        throw new InvalidOperationException("Should not be called, cause interface is just a marker");
    }
}
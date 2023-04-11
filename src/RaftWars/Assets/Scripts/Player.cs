using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Visual;
using Random = UnityEngine.Random;
using Infrastructure;

public class Player : FighterRaft, IPlatformsCarrier
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
    [FormerlySerializedAs("hpIncrease")] [SerializeField] private int hpIncomeForPeople = 5;
    [FormerlySerializedAs("damageIncrease")] [SerializeField] private int damageIncomeForPeople = 5;
    [SerializeField] public float speed;
    public CinemachineTargetGroup CameraGroup;
    [SerializeField] private GameObject[] _indicators;

    private int damageClear = 10;
    private int platformHp;
    private float hpAdditive;
    private float damageAdditive;
    public bool battle;
    private float timer = 0;
    public bool isDead;
    public int platformCount = 1;
    public bool canPlay;
    public int coins;
    public int gems;
    public int warriorsCount = 2;
    public CinemachineCameraOffset _camera;
    private float damageToPlayer;
    private const float WinningDamageCoefficient = .6f;
    private const float LoosingDamageCoefficient = .4f;
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
    public static event Action Died;

    public Vector3 MoveDirectionXZ => new(_input.Horizontal, 0, _input.Vertical);
    public float Bounds => edgesAndAngleWaves.Bounds;

    public void Construct()
    {
        edgesAndAngleWaves = gameObject.AddComponent<EdgesAndAngleWaves>();
        edgesAndAngleWaves.Construct(this);
    }
    
    private void Start()
    {
        
        _enemyHud = GameFactory.CreateEnemyHud();
        _enemyHud.transform.SetParent(Game.StatsCanvas.transform, worldPositionStays: false);
        _enemyHud.Target = transform;
        _input = Game.InputService;
        _materialsService = Game.MaterialsService;
        _hud = Game.Hud;
        
        hp = 1;
        damage = 1;
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
        if (PlayerPrefs.HasKey("PlayerNick"))
        {
            _enemyHud.nickname.text = PlayerPrefs.GetString("PlayerNick");
        }
        else
        {
            PlayerPrefs.SetString("PlayerNick", "Player" + Random.Range(1000, 9999));
            _enemyHud.nickname.text = PlayerPrefs.GetString("PlayerNick");
        }
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
    }

    public void AddCoins(int coins)
    {
        this.coins += coins;
        _hud.ShowCoins(this.coins);
    }

    public void SpendCoins(int coins)
    {
        Game.MoneyService.Spend(coins);
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

    public void DealDamage(int amount = 1)
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

    public void AddTurret(Turret tur, int damageIncrease, int healthIncrease)
    {
        AddPlatform(tur.GetComponentInParent<Platform>());
        turrets.Add(tur);
        platformHp += healthIncrease;
        hp += (int) (healthIncrease * (1 + hpAdditive));
        damage += (int) (damageIncrease * (1 + damageAdditive));
        RecountStats();
    }
    
    public void AddFastTurret(Turret tur, float speed)
    {
        AddPlatform(tur.GetComponentInParent<Platform>());
        this.speed += speed;
    }

    private void RecountStats()
    {
        _enemyHud.hpText.text = Mathf.RoundToInt(Mathf.Clamp(hp, 0, 99999)).ToString();
        _enemyHud.damageText.text = Mathf.RoundToInt(Mathf.Clamp(damage, 0, 99999)).ToString();
        warriorsCount = warriors.Count;
    }

    private void CheckHp()
    {
        bool Something()
        {
            return hp - platformHp * (1 + hpAdditive) <= (warriors.Count - 1) * hpIncomeForPeople * (1 + hpAdditive);
        }
        
        if (warriorsCount > 0)
        {
            if (Something())
            {
                if (warriors.Count > 0)
                {
                    MakeRandomPeopleDie();
                    damage -= (int) (damageIncomeForPeople * (1 + damageAdditive));
                }
            }
        }
        RecountStats();
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
        
        StartCoroutine(CreateExplosions());
    
        var virtualCamera = _camera.GetComponent<CinemachineVirtualCamera>();
        virtualCamera.m_Follow = null;
        virtualCamera.m_LookAt = CameraGroup.transform;
        transform.DOMoveY(-999, .65f).SetSpeedBased(true);
    }

    private IEnumerator CreateExplosions()
    {
        const float explosionFrequency = .5f;
        int explosionCountAtATime = 2 * Math.Clamp(platforms.Count, 1, 999);
        
        while (true)
        {
            for (var i = 0; i < explosionCountAtATime; i++)
            {
                Explosion explosion = GameFactory.CreateExplosion();
                explosion.transform.position = GetScaledRandomPointAmongAllPlatforms();
            }
            yield return new WaitForSeconds(explosionFrequency);
        }
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

    public void AddPlatform(Platform platform)
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
        OnBattleEnded();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DefaultNamespace;
using DG.Tweening;
using InputSystem;
using RaftWars.Infrastructure;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour, IPlatformsCarrier
{
    [SerializeField] private List<People> warriors;
    [SerializeField] private List<Platform> platforms;
    [SerializeField] private List<Turret> turrets = new List<Turret>();
    [SerializeField] private TextMeshPro hpText;
    [SerializeField] private TextMeshPro damageText;
    [SerializeField] private TextMeshPro nickname;
    [SerializeField] private Text coinsText, gemsText;
    [FormerlySerializedAs("maximumHp")] [FormerlySerializedAs("fullHp")] public float hp;
    [FormerlySerializedAs("maximumDamage")] [FormerlySerializedAs("fullDamage")] public float damage;
    [FormerlySerializedAs("hpIncrease")] [SerializeField] private float hpIncomeForPeople = 5;
    [FormerlySerializedAs("damageIncrease")] [SerializeField] private float damageIncomeForPeople = 5;
    [FormerlySerializedAs("battleKoef")] public float battleDamageOverTime = 1;
    [SerializeField] private float speed;
    [SerializeField] private CinemachineTargetGroup _group;
    [SerializeField] private GameObject[] _indicators;

    private float damageClear = 10;
    private float platformHp;
    private float hpAdditive;
    private float damageAdditive;
    private bool battle;
    private float timer = 0;
    public bool isDead;
    public int platformCount = 1;
    public bool canPlay;
    public int coins;
    public int gems;
    public int warriorsCount = 2;
    public CinemachineCameraOffset _camera;
    private float damageToEnemy;
    private const float WinningDamageCoefficient = .8f;
    private const float LoosingDamageCoefficient = 0.6f;

    private Enemy enemyForBattle;
    private InputService _input;
    public static Player instance;
    private Rigidbody rb;
    private MaterialsService _materialsService;
    private Material _material;
    private bool idleBehaviour;
    private Edges _edges;
    public static event Action Died;

    private void Start()
    {
        hp = 1;
        damage = 1;
        _materialsService = Game.MaterialsService;
        _material = _materialsService.GetPlayerMaterial();
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
        CreateInput();
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
        _edges = gameObject.AddComponent<Edges>();
        _edges.Construct(this, _material);
        _edges.WarmupEdges();
    }

    private void TryGenerateNickname()
    {
        if (PlayerPrefs.HasKey("PlayerNick"))
        {
            nickname.text = PlayerPrefs.GetString("PlayerNick");
        }
        else
        {
            PlayerPrefs.SetString("PlayerNick", "Player" + Random.Range(1000, 9999));
            nickname.text = PlayerPrefs.GetString("PlayerNick");
        }
    }

    private void CreateInput()
    {
        _input = gameObject.AddComponent<InputService>();
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
        foreach (var platform in enemyForBattle.platforms)  
        {
            _group.RemoveMember(platform.transform);
        }
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
        coinsText.text = this.coins.ToString();
    }

    public void SpendCoins(int coins)
    {
        this.coins -= coins;
        coinsText.text = this.coins.ToString();
    }

    public void AddGems(int gems)
    {
        this.gems += gems;
        gemsText.text = this.gems.ToString();
    }

    public void AddPeople(People warrior)
    {
        warriors.Add(warrior);
        hp += hpIncomeForPeople * (1 + hpAdditive);
        damage += damageIncomeForPeople * (1 + damageAdditive);
        warrior.Material = _material;
        RecountStats();
    }

    public void AddHealTurret(float healthIncrease)
    {
        platformHp += healthIncrease;
        hp += healthIncrease * (1 + hpAdditive);
        RecountStats();
    }

    public void AddTurret(Turret tur, float damageIncrease, float healthIncrease)
    {
        turrets.Add(tur);
        platformHp += healthIncrease;
        hp += healthIncrease * (1 + hpAdditive);
        damage += damageIncrease * (1 + damageAdditive);
        RecountStats();
    }
    
    public void AddFastTurret(Turret tur, float speed)
    {
        this.speed += speed;
    }

    private void RecountStats()
    {
        hpText.text = Mathf.RoundToInt(Mathf.Clamp(hp, 0, 99999)).ToString();
        damageText.text = Mathf.RoundToInt(Mathf.Clamp(damage, 0, 99999)).ToString();
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
                    People warrior = warriors[Random.Range(0, warriors.Count)];
                    warrior.PlayDyingAnimation();
                    warriors.Remove(warrior);
                    damage -= damageIncomeForPeople * (1 + damageAdditive);
                }
            }
        }
        RecountStats();
    }

    private void FixedUpdate()
    {
        if (_input == null)
            return;

        if (!canPlay)
        {
            rb.velocity = Vector3.zero;
            return;
        }
        
        if (!battle)
        {
            rb.velocity = new Vector3(_input.Horizontal, 0, _input.Vertical) * speed;
        }
        else
        {
            rb.velocity = Vector3.zero;
            damageToEnemy = enemyForBattle.maximumDamage * battleDamageOverTime * Time.fixedDeltaTime * WinningDamageCoefficient;
            GetDamage(damageToEnemy);
        }
    }

    public void StartBattle(Enemy enemy)
    {
        bool ControversialBattle(float f, float enemyHealth1, float enemyDamage1)
        {
            return f >= enemyHealth1 && enemyDamage1 >= hp;
        }
        
        if (battle) return;

        foreach (var platform in enemy.platforms)
        {
            _group.AddMember(platform.transform, 1, 7);
        }
        idleBehaviour = false;
        enemyForBattle = enemy;
        battle = true;
        MakeWarriorsShot(enemy);
        MakeTurretsShot(enemy);
        PushPlayerTowardsEnemy(enemy);

        float playerDamage = 0, enemyDamage = 0;
        float enemyHealth = enemy.maximumHp;
        while (true)
        {
            playerDamage += battleDamageOverTime * damage * Time.fixedDeltaTime;
            enemyDamage += battleDamageOverTime * enemy.maximumDamage * Time.fixedDeltaTime;

            if (ControversialBattle(playerDamage, enemyHealth, enemyDamage))
            {
                bool playerSuperior = playerDamage >= enemyDamage;
                WhenControversialBattle_PlayerSuperior(enemy, when: playerSuperior);
                WhenControversialBattle_EnemySuperior(enemy, when: !playerSuperior);
                break;
            }

            if (playerDamage > enemyHealth)
            {
                damageToEnemy = enemy.maximumDamage * battleDamageOverTime * Time.fixedDeltaTime * LoosingDamageCoefficient;
                enemy.AttackPlayer(damage * battleDamageOverTime * Time.fixedDeltaTime, this);
                break;
            }

            if (!(enemyDamage >= hp)) continue;
            damageToEnemy = enemy.maximumDamage * battleDamageOverTime * Time.fixedDeltaTime;
            enemy.AttackPlayer(damage * battleDamageOverTime * Time.fixedDeltaTime, this);
            break;
        }
    }

    private void WhenControversialBattle_EnemySuperior(Enemy enemy, bool when)
    {
        if (!when)
            return;
        
        damageToEnemy = enemy.maximumDamage * battleDamageOverTime * Time.fixedDeltaTime;
        float damageToPlayer = damage * battleDamageOverTime * Time.fixedDeltaTime * LoosingDamageCoefficient;
        enemy.AttackPlayer(damageToPlayer, this);
    }

    private void WhenControversialBattle_PlayerSuperior(Enemy enemy, bool when)
    {
        if (!when)
            return;
        
        damageToEnemy = enemy.maximumDamage * battleDamageOverTime * LoosingDamageCoefficient * Time.fixedDeltaTime;
        enemy.AttackPlayer(damage * battleDamageOverTime * Time.fixedDeltaTime, this);
    }

    private void PushPlayerTowardsEnemy(Enemy enemy)
    {
        rb.AddForce((transform.position - enemy.transform.position).normalized * 2, ForceMode.Impulse);
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
        PutTurretsInIdleAnimation();
        foreach (People warrior in warriors)
        {
            warrior.PlayDyingAnimation();
        }
        GameManager.instance.PlayerLost();
        Died?.Invoke();

        foreach (GameObject indicator in _indicators)
        {
            indicator.SetActive(false);
        }
        _group.m_Targets = Array.Empty<CinemachineTargetGroup.Target>();
        _camera.GetComponent<CinemachineVirtualCamera>().Follow = null;
        transform.DOMoveY(-999, 3).SetSpeedBased(true);
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

    public void GetDamage(float damage)
    {
        hp -= damage;
        CheckHp();
        if (hp <= 0)
        {
            hp = 0;
            Dead();
        }
    }

    public void AddPlatform(Platform platform)
    {
        void AddPlatformToCameraTargetGroup()
        {
            _group.AddMember(platform.transform, 1, 7);
        }

        platformCount++;
        platforms.Add(platform);
        AddPlatformToCameraTargetGroup();
        _edges.UpdateEdges(platform.gameObject);
        _camera.m_Offset.z -= 1;
    }

    public void AmplifyDamage(float percent)
    {
        damage += damage * (percent - damageAdditive);
        damageAdditive = percent;
    }

    public void IncreaseHealth(float bonus)
    {
        hp += hp * (bonus - hpAdditive);
        hpAdditive = bonus;
    }

    public IEnumerable<GameObject> GetPlatforms()
    {
        return platforms.Select(x => x.gameObject);
    }
}
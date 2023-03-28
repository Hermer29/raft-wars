using System.Collections.Generic;
using InputSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public static Player instance;
    private Rigidbody rb;

    [SerializeField] private List<People> warriors;
    [SerializeField] private List<Platform> platforms;
    [SerializeField] private List<Turret> turrets = new List<Turret>();

    [SerializeField] private TextMeshPro hpText;
    [SerializeField] private TextMeshPro damageText;
    [SerializeField] private TextMeshPro nickname;

    private float damageClear = 10;
    private float platformHp = 0;

    private float hpAdditive = 0;
    private float damageAdditive = 0;

    [FormerlySerializedAs("fullHp")] public float maximumHp;
    [FormerlySerializedAs("fullDamage")] public float maximumDamage;

    [FormerlySerializedAs("hpIncrease")] [SerializeField] private float hpIncome = 5;
    [FormerlySerializedAs("damageIncrease")] [SerializeField] private float damageIncome = 5;
    [SerializeField] private float speed;

    private bool battle = false;

    private Enemy enemyForBattle;
    private float timer = 0;
    public bool isDead = false;

    private FlyCamera flyCamera;
    public int platformCount = 1;

    public bool canPlay = false;

    public int coins = 0;
    public int gems = 0;

    public int warriorsCount = 2;

    public float battleKoef = 1;

    private float enemyDmg;
    private float tempKoef = 1;

    [SerializeField] private Text coinsText, gemsText;
    private PlayerController _input;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(instance);
            instance = this;
        }

        rb = GetComponent<Rigidbody>();
        flyCamera = GetComponentInChildren<FlyCamera>();
        RecountStats();

        if (PlayerPrefs.HasKey("PlayerNick"))
        {
            nickname.text = PlayerPrefs.GetString("PlayerNick");
        }
        else
        {
            PlayerPrefs.SetString("PlayerNick", "Player" + Random.Range(1000, 9999));
            nickname.text = PlayerPrefs.GetString("PlayerNick");
        }
        CreateInput();
    }

    private void CreateInput()
    {
        _input = gameObject.AddComponent<PlayerController>();
    }

    private void Update()
    {
        if (!battle) return;
        if (!enemyForBattle.isDead && enemyForBattle != null) return;
        battle = false;
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
        maximumHp += hpIncome * (1 + hpAdditive);
        maximumDamage += damageIncome * (1 + damageAdditive);
        RecountStats();
    }

    public void AddHealTurret(float healthIncrease)
    {
        platformHp += healthIncrease;
        maximumHp += healthIncrease * (1 + hpAdditive);
        RecountStats();
    }

    public void AddTurret(Turret tur, float damageIncrease, float healthIncrease)
    {
        turrets.Add(tur);
        platformHp += healthIncrease;
        maximumHp += healthIncrease * (1 + hpAdditive);
        maximumDamage += damageIncrease * (1 + damageAdditive);
        RecountStats();
    }
    

    public void AddFastTurret(Turret tur, float speed)
    {
        //turrets.Add(tur);
        this.speed += speed;
    }

    private void RecountStats()
    {
        hpText.text = Mathf.RoundToInt(maximumHp).ToString();
        damageText.text = Mathf.RoundToInt(maximumDamage).ToString();
        warriorsCount = warriors.Count;
    }

    private void CheckHp()
    {
        bool Something()
        {
            return maximumHp - platformHp * (1 + hpAdditive) <= (warriors.Count - 1) * hpIncome * (1 + hpAdditive);
        }
        
        if (warriorsCount > 0)
        {
            if (Something())
            {
                if (warriors.Count > 0)
                {
                    People warrior = warriors[Random.Range(0, warriors.Count)];
                    warrior.DeathAnim();
                    warriors.Remove(warrior);
                    maximumDamage -= damageIncome * (1 + damageAdditive);
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
            enemyDmg = enemyForBattle.fullDamage * battleKoef * Time.fixedDeltaTime * tempKoef;
            GetDamage(enemyDmg);
        }
    }

    public void StartBattle(Enemy enemy)
    {
        if (battle) return;
        enemyForBattle = enemy;
        battle = true;
        MakeWarriorsShot(enemy);
        MakeTurretsShot(enemy);
        rb.AddForce((transform.position - enemy.transform.position).normalized * 2, ForceMode.Impulse);

        float dmg1 = 0, dmg2 = 0;
        float enDmg = enemy.fullDamage, enHp = enemy.fullHp;
        while (true)
        {
            dmg1 += battleKoef * maximumDamage * Time.fixedDeltaTime;
            dmg2 += battleKoef * enDmg * Time.fixedDeltaTime;

            if (dmg1 >= enHp && dmg2 >= maximumHp)
            {
                if (dmg1 >= dmg2)
                {
                    tempKoef = 0.8f;
                    enemyDmg = enDmg * battleKoef * tempKoef * Time.fixedDeltaTime;
                    enemy.AttackPlayer(maximumDamage * battleKoef * Time.fixedDeltaTime, this);
                }
                else
                {
                    tempKoef = 1;
                    enemyDmg = enDmg * battleKoef * Time.fixedDeltaTime;
                    enemy.AttackPlayer(maximumDamage * battleKoef * Time.fixedDeltaTime * 0.8f, this);
                }
                break;
            }

            if (dmg1 > enHp)
            {
                tempKoef = 0.8f;
                enemyDmg = enDmg * battleKoef * Time.fixedDeltaTime * tempKoef;
                enemy.AttackPlayer(maximumDamage * battleKoef * Time.fixedDeltaTime, this);
                break;
            }

            if (!(dmg2 >= maximumHp)) continue;
            tempKoef = 1;
            enemyDmg = enDmg * battleKoef * Time.fixedDeltaTime;
            enemy.AttackPlayer(maximumDamage * battleKoef * Time.fixedDeltaTime, this);
            break;
        }
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
        GameManager.instance.Failed();
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
        maximumHp -= damage;
        CheckHp();
        if (maximumHp <= 0)
        {
            maximumHp = 0;
            Dead();
        }
    }

    public void AddPlatform(Platform platform)
    {
        platformCount++;
        if (platformCount % 4 == 0)
            flyCamera.Move();
        platforms.Add(platform);
    }

    public void AmplifyDamage(float percent)
    {
        maximumDamage += maximumDamage * (percent - damageAdditive);
        damageAdditive = percent;
    }

    public void IncreaseHealth(float bonus)
    {
        maximumHp += maximumHp * (bonus - hpAdditive);
        hpAdditive = bonus;
    }
}
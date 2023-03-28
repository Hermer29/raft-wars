using System.Collections.Generic;
using InputSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    private float platformHP = 0;

    private float hpAdditive = 0;
    private float damageAdditive = 0;

    public float fullHp;
    public float fullDamage;

    [SerializeField] private float hpIncrease = 5;
    [SerializeField] private float damageIncrease = 5;
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
        if (battle)
        {
            if (enemyForBattle.isDead || enemyForBattle == null)
            {
                battle = false;
                for (int i = 0; i < warriors.Count; i++)
                {
                    warriors[i].IdleAnim();
                }
                for (int i = 0; i < turrets.Count; i++)
                {
                    turrets[i].IdleAnim();
                }
            }
        } 
    }

    public void AddCoins(int coins)
    {
        this.coins += coins;
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
        fullHp += hpIncrease * (1 + hpAdditive);
        fullDamage += damageIncrease * (1 + damageAdditive);
        RecountStats();
    }

    public void AddTurret(Turret tur, float damageIncrease)
    {
        turrets.Add(tur);
        fullDamage += damageIncrease * (1 + damageAdditive);
        RecountStats();
    }

    public void AddHealTurret(float healthIncrease)
    {
        platformHP += healthIncrease;
        fullHp += healthIncrease * (1 + hpAdditive);
        RecountStats();
    }

    public void AddTurret(Turret tur, float damageIncrease, float healthIncrease)
    {
        turrets.Add(tur);
        platformHP += healthIncrease;
        fullHp += healthIncrease * (1 + hpAdditive);
        fullDamage += damageIncrease * (1 + damageAdditive);
        RecountStats();
    }
    

    public void AddFastTurret(Turret tur, float speed)
    {
        //turrets.Add(tur);
        this.speed += speed;
    }

    private void RecountStats()
    {
        hpText.text = Mathf.RoundToInt(fullHp).ToString();
        damageText.text = Mathf.RoundToInt(fullDamage).ToString();
        warriorsCount = warriors.Count;
    }

    private void CheckHP()
    {
        if (warriorsCount > 0)
        {
            if (fullHp - platformHP * (1 + hpAdditive) <= (warriors.Count - 1) * hpIncrease * (1 + hpAdditive))
            {
                if (warriors.Count > 0)
                {
                    People warrior = warriors[Random.Range(0, warriors.Count)];
                    warrior.DeathAnim();
                    warriors.Remove(warrior);
                    fullDamage -= damageIncrease * (1 + damageAdditive);
                }
            }
        }
        RecountStats();
    }

    private void FixedUpdate()
    {
        if (_input == null)
            return;
        
        if (canPlay)
        {
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
    }

    public void StartBattle(Enemy enemy)
    {
        if (!battle)
        {
            enemyForBattle = enemy;
            battle = true;
            for (int i = 0; i < warriors.Count; i++)
            {
                warriors[i].ShootAnim(enemy.transform);
            }
            for (int i = 0; i < turrets.Count; i++)
            {
                turrets[i].ShootAnim(enemy.transform);
            }
            rb.AddForce((transform.position - enemy.transform.position).normalized * 2, ForceMode.Impulse);

            float dmg1 = 0, dmg2 = 0;
            float enDmg = enemy.fullDamage, enHP = enemy.fullHp;
            while (true)
            {
                dmg1 += battleKoef * fullDamage * Time.fixedDeltaTime;
                dmg2 += battleKoef * enDmg * Time.fixedDeltaTime;

                if (dmg1 >= enHP && dmg2 >= fullHp)
                {
                    if (dmg1 >= dmg2)
                    {
                        tempKoef = 0.8f;
                        enemyDmg = enDmg * battleKoef * tempKoef * Time.fixedDeltaTime;
                        enemy.GetBattlePrefs(fullDamage * battleKoef * Time.fixedDeltaTime, this);
                    }
                    else
                    {
                        tempKoef = 1;
                        enemyDmg = enDmg * battleKoef * Time.fixedDeltaTime;
                        enemy.GetBattlePrefs(fullDamage * battleKoef * Time.fixedDeltaTime * 0.8f, this);
                    }
                    break;
                }
                else if (dmg1 > enHP)
                {
                    tempKoef = 0.8f;
                    enemyDmg = enDmg * battleKoef * Time.fixedDeltaTime * tempKoef;
                    enemy.GetBattlePrefs(fullDamage * battleKoef * Time.fixedDeltaTime, this);
                    break;
                }
                else if (dmg2 >= fullHp)
                {
                    tempKoef = 1;
                    enemyDmg = enDmg * battleKoef * Time.fixedDeltaTime;
                    enemy.GetBattlePrefs(fullDamage * battleKoef * Time.fixedDeltaTime, this);
                    break;
                }
            }
        }
    }

    private void Dead()
    {
        isDead = true;
        canPlay = false;
        for (int i = 0; i < turrets.Count; i++)
        {
            turrets[i].IdleAnim();
        }
        GameManager.instance.Failed();
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
        fullHp -= damage;
        CheckHP();
        if (fullHp <= 0)
        {
            fullHp = 0;
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

    public void AddDamageBonus(float bonus)
    {
        fullDamage += fullDamage * (bonus - damageAdditive);
        damageAdditive = bonus;
    }

    public void AddHpBonus(float bonus)
    {
        fullHp += fullHp * (bonus - hpAdditive);
        hpAdditive = bonus;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Enemy : MonoBehaviour
{
    [SerializeField] private List<Material> colorMaterials;

    private List<People> warriors = new List<People>();
    [SerializeField] private List<Platform> platforms = new List<Platform>();
    private List<Turret> turrets = new List<Turret>();
    private int warriorsCount;

    [SerializeField] private TextMeshPro hpText;
    [SerializeField] private TextMeshPro damageText;
    [SerializeField] private TextMeshPro nickname;

    private float hpClear = 0;
    private float damageClear = 0;
    private float turretDamage = 0;
    private float platformHP = 0;

    public float fullHp;
    public float fullDamage;

    [SerializeField] private float hpIncrease = 5;
    [SerializeField] private float damageIncrease = 5;

    public bool battle = false;

    private Player player;
    private float timer = 0;
    public bool isDead = false;
    public bool isBoss = false;

    private List<PeopleAdditive> peopleAdditive = new List<PeopleAdditive>();
    private List<PlatformAdditive> platformsAdditive = new List<PlatformAdditive>();

    private Vector3 prevSpawnPoint;
    public bool boss5Stage = true;

    private float playerDmg;

    public bool hasShield = false;
    public List<GameObject> enemiesToKill;
    public GameObject shieldToOff;

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

        if (!battle) return;
        
        if (!player.isDead && player != null) return;
        battle = false;
        timer = 0;
        PlayIdleAnimation();
    }

    private void PlayIdleAnimation()
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

    private void FixedUpdate()
    {
        if (battle)
        {
            GetDamage(playerDmg);
        }
    }

    private void Start()
    {
        if (!isBoss)
        {
            nickname.text = "Player" + Random.Range(1000, 10000);
        }
        if (boss5Stage)
        {
            Material mat = colorMaterials[Random.Range(0, colorMaterials.Count)];
            for (int i = 0; i < platforms.Count; i++)
            {
                platforms[i].isEnemy = true;
                platforms[i].colorMat = mat;
                platforms[i].gameObject.layer = LayerMask.NameToLayer("Enemy");
                if (platforms[i].isTurret)
                {
                    turrets.Add(platforms[i].GetComponentInChildren<Turret>());
                    turretDamage += platforms[i].GetComponentInChildren<Turret>().damageIncrease;
                    platformHP += platforms[i].GetComponentInChildren<Turret>().healthIncrease;
                    platforms[i].GetComponentInChildren<Turret>().DrawInMyColor(mat);
                }
            }

            foreach (People c in GetComponentsInChildren<People>())
            {
                platforms[0].TakePeople(c.gameObject);
            }

            RecountStats();
        }
    }

    public void Spawn(IEnumerable<Platform> platforms, People[] people, int hp, int damage, List<PlatformAdditive> platformsAdd, List<PeopleAdditive> peopleAdd)
    {
        if (boss5Stage) return;
        Material mat = colorMaterials[Random.Range(0, colorMaterials.Count)];
        platformsAdditive = platformsAdd;
        peopleAdditive = peopleAdd;
        this.platforms[0].isEnemy = true;
        this.platforms[0].colorMat = mat;
        this.platforms[0].gameObject.layer = LayerMask.NameToLayer("Enemy");
        Vector3 startPoint = transform.position;
        hpIncrease = hp;
        damageIncrease = damage;
        warriorsCount = people.Length;
        prevSpawnPoint = startPoint;
        foreach (Platform platform in platforms)
        {
            while (true)
            {
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    if (Random.Range(0f, 1f) > 0.5f)
                        startPoint.x += 3;
                    else
                        startPoint.x -= 3;
                }
                else
                {
                    if (Random.Range(0f, 1f) > 0.5f)
                        startPoint.z += 3;
                    else
                        startPoint.z -= 3;
                }

                var outCols = Physics.OverlapSphere(startPoint, 1);
                if (outCols == null || outCols.Length == 0)
                {
                    break;
                }
                startPoint = prevSpawnPoint;
            }
            Platform plat = Instantiate(platform, startPoint, Quaternion.identity);
            plat.isEnemy = true;
            plat.colorMat = mat;
            plat.gameObject.layer = LayerMask.NameToLayer("Enemy");
            plat.transform.parent = gameObject.transform;
            if (plat.ishospital)
            {
                platformHP += plat.GetComponentInChildren<Turret>().healthIncrease;
                fullHp += plat.GetComponentInChildren<Turret>().healthIncrease;
            }
            else if (plat.isTurret)
            {
                turrets.Add(plat.GetComponentInChildren<Turret>());
                turretDamage += plat.GetComponentInChildren<Turret>().damageIncrease;
                fullDamage += plat.GetComponentInChildren<Turret>().damageIncrease;
                platformHP += plat.GetComponentInChildren<Turret>().healthIncrease;
                fullHp += plat.GetComponentInChildren<Turret>().healthIncrease;
                platform.GetComponentInChildren<Turret>().DrawInMyColor(mat);
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

    private void RecountStats()
    {
        hpText.text = Mathf.RoundToInt(fullHp).ToString();
        damageText.text = Mathf.RoundToInt(fullDamage).ToString();
    }

    public void AddPeople(People warrior)
    {
        warriors.Add(warrior);
        fullHp += hpIncrease;
        fullDamage += damageIncrease;
        RecountStats();
    }

    public void StartBattle(Player target)
    {
        player = target;
        battle = true;
        PlayShotAnimation(target);
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

    public void AttackPlayer(float dmg, Player target)
    {
        playerDmg = dmg;
        player = target;
        battle = true;
        
        PlayShotAnimation(target);
    }

    public void Dead()
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
        for (int i = 0; i < platformsAdditive.Count; i++)
        {
            if (Random.Range(0f, 1f) > 0.7f)
            {
                Instantiate(platformsAdditive[i], pos, Quaternion.identity);
                if (Random.Range(0, 2) == 0)
                    pos.x += Random.Range(5, 10);
                else
                    pos.x -= Random.Range(5, 10);

                if (Random.Range(0, 2) == 0)
                    pos.z += Random.Range(5, 10);
                else
                    pos.z -= Random.Range(5, 10);
            }
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
                Instantiate(peopleAdditive[Random.Range(0, peopleAdditive.Count)], pos, Quaternion.identity); ;
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

        for(int i = 0; i < warriors.Count; i++)
        {
            warriors[i].DeathAnim();
            warriors.RemoveAt(i);
            damageClear -= damageIncrease;
            damageText.text = damageClear.ToString();
        }

        Destroy(gameObject);
    }

    private void CheckHP()
    {
        if (warriorsCount > 0)
        {
            if (fullHp - platformHP <= (warriors.Count - 1) * hpIncrease )
            {
                if (warriors.Count > 0)
                {
                    People warrior = warriors[Random.Range(0, warriors.Count)];
                    warrior.DeathAnim();
                    warriors.Remove(warrior);
                    fullDamage -= damageIncrease;
                }
            }
        }
        RecountStats();
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
}

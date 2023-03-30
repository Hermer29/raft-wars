using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DefaultNamespace;
using InputSystem;
using RaftWars.Infrastructure;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.UI;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;


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
    [FormerlySerializedAs("fullDamage")] public float maximumDamage;
    [SerializeField] private float hpIncrease = 5;
    [SerializeField] private float damageIncrease = 5;
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
    private const float SqrMagnitudeDistanceToReactOnPlayer = 10 * 10;
    
    private int StatsSum => (int) (fullHp + maximumDamage);

    private void Start()
    {
        _player = Game.PlayerService;
        TryGenerateNickname(when: !isBoss);

        if (!boss5Stage) return;
        WarmupPlatforms();
        AssignRelatedPeople();
        RecountStats();
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
        Material mat = colorMaterials[Random.Range(0, colorMaterials.Count)];
        foreach (Platform platform in platforms)
        {
            platform.isEnemy = true;
            platform.colorMat = mat;
            platform.gameObject.layer = LayerMask.NameToLayer("Enemy");
            if (!platform.isTurret) continue;
            turrets.Add(platform.GetComponentInChildren<Turret>());
            turretDamage += platform.GetComponentInChildren<Turret>().damageIncrease;
            platformHP += platform.GetComponentInChildren<Turret>().healthIncrease;
            platform.GetComponentInChildren<Turret>().DrawInMyColor(mat);
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
        PlayIdleAnimation();
    }

    private void TryMoveEnemy(float deltaTime)
    {
        if (_player.GameStarted == false)
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

        if ((_player.Position - transform.position).sqrMagnitude < SqrMagnitudeDistanceToReactOnPlayer)
        {
            bool playerSuperior = _player.PlayerStatsSum >= StatsSum;
            if (playerSuperior)
            {
                if(ReachedWorldBounds())
                {
                    MoveInRandomDirection();
                    goto Exit;
                }

                _moveDirection = -(_player.Position - transform.position).normalized;
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

        var minimal = Mathf.Min(distanceToZMaxBound, distanceToZMinBound, distanceToXMinBound, distanceToXMaxBound);
        Debug.Log($"{name} Nearest bound distance: {minimal}");
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

        Debug.Log($"{name} Nearest bound position {result}");
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
            
            Debug.Log($"{gameObject.name} reached world bounds. normal back");
            return Vector3.back;
        }

        if (distanceToZMinBound == minimal)
        {
            
            Debug.Log($"{gameObject.name} reached world bounds. normal forward");
            return Vector3.forward;
        }

        if (distanceToXMinBound == minimal)
        {
            
            Debug.Log($"{gameObject.name} reached world bounds. normal right");
            return Vector3.right;
        }

        if (distanceToXMaxBound == minimal)
        {
            Debug.Log($"{gameObject.name} reached world bounds. normal left");
            return Vector3.left;
        }

        throw new Exception("Unreachable");
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

    public void SpawnEnvironment(IEnumerable<Platform> platforms, People[] people, int hp, int damage, List<AttachablePlatform> platformsAdd, List<PeopleThatCanBeTaken> peopleAdd)
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
                maximumDamage += plat.GetComponentInChildren<Turret>().damageIncrease;
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
        damageText.text = Mathf.RoundToInt(maximumDamage).ToString();
    }

    public void AddPeople(People warrior)
    {
        warriors.Add(warrior);
        fullHp += hpIncrease;
        maximumDamage += damageIncrease;
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
            warriors[i].PlayDyingAnimation();
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
        fullHp -= damage;
        CheckHP();
        if (fullHp <= 0)
        {
            fullHp = 0;
            Dead();
        }
    }
}

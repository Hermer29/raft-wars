using System;
using DefaultNamespace;
using Skins.Platforms;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class Platform : MonoBehaviour, ICanTakePeople, ICanTakePlatform, ICanTakeCoins, ICanTakeGems, ICanTakeBarrel
{
    public bool isEnemy;
    private bool battle = false;
    public bool isTurret;
    public bool isWind;
    public bool ishospital;
    private Enemy _relatedEnemy;
    private Material _material;
    private PlatformSkin _skin;
    private MeshRenderer _meshRenderer;

    public Material Material
    {
        set
        {
            _material = value;
            var turret = GetComponentInChildren<Turret>();
            turret?.DrawInMyColor(_material);
        }
        get => _material;
    }

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        _relatedEnemy = GetComponentInParent<Enemy>();
    }

    public void TakePeople(GameObject warrior)
    {
        Assert.IsNotNull(warrior);
        
        if (!isTurret)
        {
            Vector3 spawnPoint = transform.position;
            spawnPoint = FindPointOnPlatform(spawnPoint);
            People people = Instantiate(warrior, spawnPoint, Quaternion.identity, transform).GetComponent<People>();
            
            if (transform.parent.GetComponent<Player>() != null)
            {
                transform.parent.GetComponent<Player>().AddPeople(people);
            }
            else
            {
                transform.parent.GetComponent<Enemy>().AddPeople(people);
            }
            people.SetColor(_material);
            people.SetRelatedPlatform(this);
        }
        else
        {
            FigureOutWhereSpawnPeopleInstead(warrior);
        }
    }

    private void FigureOutWhereSpawnPeopleInstead(GameObject warrior)
    {
        Platform platform = GetComponentInParent<Player>().GetPlatformWithoutTurret();
        Vector3 spawnPoint = platform.transform.position;
        spawnPoint = FindPointOnPlatform(spawnPoint);
        People people = Instantiate(warrior, spawnPoint, Quaternion.identity, platform.transform).GetComponent<People>();
        if (transform.parent.GetComponent<Player>() != null)
        {
            transform.parent.GetComponent<Player>().AddPeople(people.GetComponent<People>());
        }
        else
        {
            transform.parent.GetComponent<Enemy>().AddPeople(people.GetComponent<People>());
        }

        people.SetColor(_material);
        people.SetRelatedPlatform(this);
    }

    public static Vector3 FindPointOnPlatform(Vector3 spawnPoint)
    {
        spawnPoint.x += Random.Range(-Constants.PlatformSize / 2.4f, Constants.PlatformSize / 2.4f);
        spawnPoint.z += Random.Range(-Constants.PlatformSize / 2.4f, Constants.PlatformSize / 2.4f);
        spawnPoint.y += 0.5f;
        return spawnPoint;
    }

    public Vector3 FindPointOnPlatform()
    {
        Vector3 middle = transform.position;
        return FindPointOnPlatform(middle);
    }

    public void TakePlatform(GameObject platform, Vector3 pos)
    {
        if (isEnemy) return;

        Vector3 spawnPos = transform.position;
        Vector3 vectorFromPlayer = pos - transform.position;
        if (Mathf.Abs(vectorFromPlayer.x) > Mathf.Abs(vectorFromPlayer.z))
        {
            if (vectorFromPlayer.x > 0)
                spawnPos.x += Constants.PlatformSize;
            else
                spawnPos.x -= Constants.PlatformSize;
        }
        else
        {
            if (vectorFromPlayer.z > 0)
                spawnPos.z += Constants.PlatformSize;
            else
                spawnPos.z -= Constants.PlatformSize;
        }
        
        GameObject platformInstance = Instantiate(platform, spawnPos, Quaternion.identity, transform.parent);
        var platformComponent = platformInstance.GetComponent<Platform>();
        platformComponent.Material = _material;
        if(_skin != null)
            platformComponent.ApplySkin(_skin);

        if (GetComponentInParent<Player>() != null)
        {
            if (platformInstance.GetComponent<Platform>().isTurret)
            {
                platformInstance.GetComponentInChildren<Turret>().DrawInMyColor(_material);
                if (!platformInstance.GetComponent<Platform>().isWind)
                    GetComponentInParent<Player>().AddTurret(platformInstance.GetComponentInChildren<Turret>(),
                        platformInstance.GetComponentInChildren<Turret>().damageIncrease,
                        platformInstance.GetComponentInChildren<Turret>().healthIncrease);
                else
                    GetComponentInParent<Player>().AddFastTurret(platformInstance.GetComponentInChildren<Turret>(), platformInstance.GetComponentInChildren<Turret>().millSpeed);
            }
            GetComponentInParent<Player>().AddPlatform(platformInstance.GetComponent<Platform>());
            platformInstance.layer = LayerMask.NameToLayer("Player");
        }
        else
            platformInstance.layer = LayerMask.NameToLayer("Enemy");
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool IsNotInBattle()
        {
            return !_relatedEnemy.battle;
        }
        if (!isEnemy) return;
        if (collision.gameObject.TryGetComponent(out Player player) == false) return;
        if (!IsNotInBattle()) return;
        var enemy = GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            if (enemy.isDead)
                return;
        }
        player.StartBattle(_relatedEnemy);
    }

    public void TakeCoins(int coins)
    {
        if (GetComponentInParent<Player>() != null)
        {
            GetComponentInParent<Player>().AddCoins(coins);
        }
    }

    public void TakeGems(int gems)
    {
        if (GetComponentInParent<Player>() != null)
        {
            GetComponentInParent<Player>().AddGems(gems);
        }
    }

    public void TakeBarrel(int damage)
    {
        if(GetComponentInParent<Player>() != null)
        {
            GetComponentInParent<Player>().DealDamage(damage);
        }
    }

    public void ApplySkin(PlatformSkin skin)
    {
        if(_skin != null)
            Destroy(_skin.gameObject);
        
        _skin = Instantiate(skin, transform);
        _meshRenderer.enabled = false;
    }

    public static Vector3 GetRandomPoint(float platformSizeModifier = 5)
    {
        return new Vector3(
            Random.Range(-Constants.PlatformSize / platformSizeModifier, Constants.PlatformSize / platformSizeModifier),
            .5f,
            Random.Range(-Constants.PlatformSize / platformSizeModifier, Constants.PlatformSize / platformSizeModifier));
    }

    public Vector3 GetRandomPoint()
    {
        return transform.position + Platform.GetRandomPoint();
    }
}

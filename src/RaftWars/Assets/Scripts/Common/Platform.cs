using Common;
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
    public bool emptyPlatform;
    private Enemy _relatedEnemy;
    private Material _material;
    private PlatformSkin _skin;
    private MeshRenderer _meshRenderer;
    public int Capacity {get; set;}

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

    public bool TryTakePeople(GameObject warrior)
    {
        Assert.IsNotNull(warrior);

        if (TryGetFighterRaftImplementation<ICanTakePeople>(out _) == false)
        {
            return false;
        }
        
        if (!isTurret && !ishospital && !isWind)
        {
            if(Capacity == 4)
            {
                return TryFigureOutWhereSpawnPeopleInstead(warrior);
            }
            Capacity++;
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
            people.SetRelatedPlatform(this);
            people.Material = _material;
        }
        else
        {
            return TryFigureOutWhereSpawnPeopleInstead(warrior);
        }
        return true;
    }

    private bool TryFigureOutWhereSpawnPeopleInstead(GameObject warrior)
    {
        var player = GetComponentInParent<Player>();
        var enemy = GetComponentInParent<Enemy>();
        Platform platform;
        if(player != null)
        {
            if(player.TryFindNotFullPlatform(out platform) == false)
            {
                return false;
            }
        }
        else
        {
            if(enemy.TryFindNotFullPlatform(out platform) == false)
            {
                return false;
            }
        }
        
        Vector3 spawnPoint = platform.transform.position;
        spawnPoint = FindPointOnPlatform(spawnPoint);
        var people = Instantiate(warrior, spawnPoint, Quaternion.identity, platform.transform)
            .GetComponent<People>();
        if (player != null)
        {
            player.AddPeople(people);
        }
        else
        {
            enemy.AddPeople(people);
        }
        platform.Capacity++;
        people.Material = _material;
        people.SetRelatedPlatform(this);
        return true;
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

    private Vector3 GetSpawnPoint(Vector3 pos)
    {
         Collider[] outCols;
            Vector3 spawnPos = transform.position;
            Vector3 diff = pos - transform.position;
            if (Mathf.Abs(diff.x) > Mathf.Abs(diff.z))
            {
                if (diff.x > 0)
                    spawnPos.x += Constants.PlatformSize;
                else
                    spawnPos.x -= Constants.PlatformSize;
            }
            else
            {
                if (diff.z > 0)
                    spawnPos.z += Constants.PlatformSize;
                else
                    spawnPos.z -= Constants.PlatformSize;
            }
            outCols = Physics.OverlapSphere(spawnPos, 1.2f);
            if (outCols.Length != 0)
            {
                var raft = GetComponentInParent<FighterRaft>();
                if(raft.PlatformsCount == 1)
                    return spawnPos;
                var trials = 0; 
                while (true)
                {
                    trials++;
                    if (trials == 5)
                        break;
                    spawnPos = raft.GetAnotherPlatform().transform.position;
                    if (Mathf.Abs(diff.x) > Mathf.Abs(diff.z))
                    {
                        if (diff.x > 0)
                            spawnPos.x += Constants.PlatformSize;
                        else
                            spawnPos.x -= Constants.PlatformSize;
                    }
                    else
                    {
                        if (diff.z > 0)
                            spawnPos.z += Constants.PlatformSize;
                        else
                            spawnPos.z -= Constants.PlatformSize;
                    }

                    outCols = Physics.OverlapSphere(spawnPos, 1.2f);
                    if (outCols.Length == 0)
                    {
                        break;
                    }
                }
            }
            return spawnPos;
    }

    public void TakePlatform(GameObject platformPrefab, Vector3 pos)
    {
        var spawnPos = GetSpawnPoint(pos);
        
        GameObject platformObject = Instantiate(platformPrefab, spawnPos, Quaternion.identity, transform.parent);
        var platformComponent = platformObject.GetComponent<Platform>();
        platformComponent.isEnemy = isEnemy;
        platformComponent.Material = _material;
        platformComponent.Capacity = 0;
        if(_skin != null)
            platformComponent.ApplySkin(_skin);
        var fighterRaft = GetComponentInParent<FighterRaft>();
        fighterRaft.AddAbstractPlatform(platformComponent, _material);
        platformObject.layer = isEnemy ? LayerMask.NameToLayer("Enemy") : LayerMask.NameToLayer("Player");
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

    public void TakeGems(int gems)
    {
        if (GetComponentInParent<Player>() != null)
        {
            GetComponentInParent<Player>().AddGems(gems);
        }
    }

    public bool TryTakeBarrel(int damage) =>
        TryGetFighterRaftImplementation(out ICanTakeBarrel barrelTaker) 
            && barrelTaker.TryTakeBarrel(damage);

    private bool TryGetFighterRaftImplementation<TImplementation>(out TImplementation implementation) where TImplementation: class
    {
        implementation = null;
        var fighter = GetComponentInParent<FighterRaft>();
        if (fighter is TImplementation checkedImplementation)
        {
            implementation = checkedImplementation;
            return true;
        }
        return false;
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

    public bool TryTakeCoins(int coins) =>
        TryGetFighterRaftImplementation(out ICanTakeCoins barrelTaker) 
            && barrelTaker.TryTakeCoins(coins);
}

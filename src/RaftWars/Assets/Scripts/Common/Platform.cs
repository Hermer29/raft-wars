using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class Platform : MonoBehaviour, ICanTakePeople, ICanTakePlatform, ICanTakeCoins, ICanTakeGems, ICanTakeBarrel
{
    private float size = 3;
    public Material colorMat;
    public bool isEnemy;
    private bool battle = false;
    public bool isTurret;
    public bool isWind;
    public bool ishospital;
    private Enemy _relatedEnemy;

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
            spawnPoint.x += Random.Range(-size / 2.4f, size / 2.4f);
            spawnPoint.z += Random.Range(-size / 2.4f, size / 2.4f);
            spawnPoint.y += 0.5f;
            People people = Instantiate(warrior, spawnPoint, Quaternion.identity).GetComponent<People>();
            people.transform.parent = transform;
            if (transform.parent.GetComponent<Player>() != null)
                transform.parent.GetComponent<Player>().AddPeople(people.GetComponent<People>());
            else
            {
                transform.parent.GetComponent<Enemy>().AddPeople(people.GetComponent<People>());
            }
            people.GetComponent<People>().SetColor(colorMat);
        }
        else
        {
            Platform _platform = GetComponentInParent<Player>().GetPlatformWithoutTurret();
            Vector3 spawnPoint = _platform.transform.position;
            spawnPoint.x += Random.Range(-size / 2.4f, size / 2.4f);
            spawnPoint.z += Random.Range(-size / 2.4f, size / 2.4f);
            spawnPoint.y += 0.5f;
            People people = Instantiate(warrior, spawnPoint, Quaternion.identity).GetComponent<People>();
            people.transform.parent = _platform.transform;
            if (transform.parent.GetComponent<Player>() != null)
                transform.parent.GetComponent<Player>().AddPeople(people.GetComponent<People>());
            else
            {
                transform.parent.GetComponent<Enemy>().AddPeople(people.GetComponent<People>());
            }
            people.GetComponent<People>().SetColor(colorMat);
        }
    }

    public void TakePlatform(GameObject platform, Vector3 pos)
    {
        if (isEnemy) return;
        
        Collider[] outCols;
        Vector3 spawnPos = transform.position;
        Vector3 diff = pos - transform.position;
        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.z))
        {
            if (diff.x > 0)
                spawnPos.x += size;
            else
                spawnPos.x -= size;
        }
        else
        {
            if (diff.z > 0)
                spawnPos.z += size;
            else
                spawnPos.z -= size;
        }
        outCols = Physics.OverlapSphere(spawnPos, 1.2f);
        if (outCols.Length != 0)
        {
            while (true)
            {
                spawnPos = GetComponentInParent<Player>().GetAnotherPlatform().transform.position;
                if (Mathf.Abs(diff.x) > Mathf.Abs(diff.z))
                {
                    if (diff.x > 0)
                        spawnPos.x += size;
                    else
                        spawnPos.x -= size;
                }
                else
                {
                    if (diff.z > 0)
                        spawnPos.z += size;
                    else
                        spawnPos.z -= size;
                }

                outCols = Physics.OverlapSphere(spawnPos, 1.2f);
                if (outCols.Length == 0)
                {
                    break;
                }
            }
        }
        GameObject _platform = Instantiate(platform, spawnPos, Quaternion.identity);
        _platform.transform.parent = transform.parent;
        _platform.GetComponent<Platform>().colorMat = colorMat;

        if (GetComponentInParent<Player>() != null)
        {
            if (_platform.GetComponent<Platform>().isTurret)
            {
                _platform.GetComponentInChildren<Turret>().DrawInMyColor(colorMat);
                if (!_platform.GetComponent<Platform>().isWind)
                    GetComponentInParent<Player>().AddTurret(_platform.GetComponentInChildren<Turret>(),
                        _platform.GetComponentInChildren<Turret>().damageIncrease,
                        _platform.GetComponentInChildren<Turret>().healthIncrease);
                else
                    GetComponentInParent<Player>().AddFastTurret(_platform.GetComponentInChildren<Turret>(), _platform.GetComponentInChildren<Turret>().millSpeed);
            }
            GetComponentInParent<Player>().AddPlatform(_platform.GetComponent<Platform>());
            _platform.layer = LayerMask.NameToLayer("Player");
                
        }
        else
            _platform.layer = LayerMask.NameToLayer("Enemy");
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool IsNotInBattle()
        {
            return !_relatedEnemy.battle;
        }
        
        if (!isEnemy) return;
        if (collision.gameObject.TryGetComponent(out Player player) == false) return;
        if (IsNotInBattle())
        {
            player.StartBattle(_relatedEnemy);
        }
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
            GetComponentInParent<Player>().GetDamage(damage);
        }
    }
}

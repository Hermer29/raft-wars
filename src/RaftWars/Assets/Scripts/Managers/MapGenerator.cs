using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InputSystem;
using RaftWars.Infrastructure;
using RaftWars.Pickables;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [FormerlySerializedAs("enemy")]
    [Header("Enemy")]
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private Platform[] enemiesToSpawn1;
    [SerializeField] private AttachablePlatform[] enemiesToSpawn1Add;
    [SerializeField] private Platform[] enemiesToSpawn2;
    [SerializeField] private AttachablePlatform[] enemiesToSpawn2Add;
    [SerializeField] private Platform[] enemiesToSpawn3;
    [SerializeField] private AttachablePlatform[] enemiesToSpawn3Add;
    [SerializeField] private Platform[] enemiesToSpawn4;
    [SerializeField] private AttachablePlatform[] enemiesToSpawn4Add;
    [SerializeField] private Platform[] enemiesToSpawn5;
    [SerializeField] private AttachablePlatform[] enemiesToSpawn5Add;
    [SerializeField] private int[] enemiesNumber;
    [SerializeField] private int[] enemyPeopleNumber;
    [SerializeField] private int[] hpIncrease;
    [SerializeField] private int[] damageIncrease;
    [SerializeField] private int[] enemyPlatformsNumber;
    [SerializeField] private People[] enemyPeopleToSpawn;
    [SerializeField] private PeopleThatCanBeTaken[] enemyPeopleToSpawnAdd;
    [SerializeField] private Enemy[] bosses;

    [Header("Platforms")]
    [SerializeField] private AttachablePlatform[] platformsToSpawn;
    [SerializeField] private int[] platformsNumber;
    
    [Header("People")]
    [SerializeField] private PeopleThatCanBeTaken[] peopleToSpawn;
    [SerializeField] private int[] peopleNumber;

    [Header("Props")]
    [SerializeField] private Coins coinsToSpawn;
    [SerializeField] private int coinsNumber;
    [SerializeField] private Gems gemsToSpawn;
    [SerializeField] private int gemsNumber;
    [SerializeField] private Barrel barrelsToSpawn;
    [SerializeField] private int barrelNumbers;

    [Header("Other Prefs")]    
    [SerializeField] private float xBorderMax, xBorderMin, yBorderMax, yBorderMin;

    public static Action<Enemy> BossCreated;

    private bool _diamondsEnabled;
    private int stage = 1;
    private MaterialsService _materials;
    private CollectiblesService _collectibles;
    private PlayerService _player;
    private Coroutine _coroutine;
    private BossAppearing _appearing;

    public void Construct(BossAppearing appearing)
    {
        _appearing = appearing;
        _diamondsEnabled = Game.FeatureFlags.DiamondsEnabledInGame;
        _collectibles = Game.CollectiblesService;
        _materials = Game.MaterialsService;
        _player = Game.PlayerService;
    }

    private void Start()
    {
        Game.GameManager.GameStarted += () => StartCoroutine(Counter());
    }

    private int _counter;
    private int _offscreenGenerationCooldown;
    
    private IEnumerator Counter()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            _counter += 2;
            if(_offscreenGenerationCooldown > 0)
                _offscreenGenerationCooldown--;
        }
    }

    public void PickedUp()
    {
        _counter = 0;
    }

    private IEnumerator SpawnOnPlayersPathRandom()
    {
        Camera camera = Camera.main;
        int probabilityToSpawnPlatformInPercent = 0;
        
        while(true)
        {
            if (_player == null)
            {
                yield return null;
                continue;
            }
            if (_player.MoveDirectionXZ.magnitude < 0.1f)
            {
                yield return null;
                continue;
            }
            Vector3 direction = _player.MoveDirectionXZ;
            direction.y = direction.z;
            const float heightOverTheCollider = 4.5f;
            var center = new Vector2(.5f, .5f);
            const float generationDistance = 2;
            Vector2 viewportPoint = direction.normalized * generationDistance * center + center;
            Ray ray = camera.ViewportPointToRay(viewportPoint);
            int raycastPlane = LayerMask.GetMask("Water");
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, raycastPlane))
            {
                Debug.Log($"Spawned collectable on way");
                Vector3 spawnPoint = hit.point + hit.normal * heightOverTheCollider;
                IEnumerable<Pickable> pickables;
                if(Random.Range(0, 100) > 80f - probabilityToSpawnPlatformInPercent)
                {
                    pickables = platformsToSpawn;
                    probabilityToSpawnPlatformInPercent = 0;
                }
                else
                {
                    probabilityToSpawnPlatformInPercent += 10;
                    pickables = peopleToSpawn.Cast<Pickable>();
                }
                Pickable prefabToSpawn = pickables.ElementAt(Random.Range(0, pickables.Count()));
                Pickable people = Instantiate(prefabToSpawn, spawnPoint, Quaternion.identity);
                people.name = "GeneratedOverTime";
            }
            else
            {
                yield return null;
                continue;
            }

            _offscreenGenerationCooldown = 30;
            yield return new WaitWhile(() => _offscreenGenerationCooldown - _counter > 0);
            Debug.Log(
                $"Offscreen generation cooldown passed. Cooldown seconds remains: {_offscreenGenerationCooldown}, Decreasing counter: {_counter}");
            _counter = 0;
        }
    }

    public void Generate(int stage)
    {
        this.stage = stage;
        FigureOutPlatformsAndEnemies(stage); 
        CreatePickablePlatforms(stage);
        CreatePickablePeople(stage);
        CreateCoinChests(stage);
        CreateGems(stage);
        CreateBarrels(stage);
    }

    private void OnEnable()
    {
        _coroutine = StartCoroutine(SpawnOnPlayersPathRandom());
    }

    private void OnDestroy()
    {
        if(_coroutine != null)
            StopCoroutine(_coroutine);
    }

    private void FigureOutPlatformsAndEnemies(int stage)
    {
        for (var i = 0; i < enemiesNumber[stage - 1];)
        {
            Vector3 posToSpawn = GetRandomSpawnPosition();
            var intersections = Physics.SphereCastAll(posToSpawn, 20, Vector3.up);
            if(intersections.Any(x => x.transform.TryGetComponent<Platform>(out var platform) && platform.isEnemy == false))
            {
                continue;
            }
            Enemy enemy = Instantiate(enemyPrefab, posToSpawn, Quaternion.identity);
            var platforms = ComeUpWithPeopleAndPlatformsCount(stage, out var people, out var pickablePeople
                ,out var pickablePlatforms);
            enemy.Material = _materials.GetRandom();
            enemy.SpawnEnvironment(platforms.ToArray(), people.ToArray(), hpIncrease[stage - 1],
                damageIncrease[stage - 1], pickablePlatforms, pickablePeople);
            GameManager.instance.AddEnemy(enemy);
            i++;
        }
    }

    private void SpawnMiscellaneous()
    {
        CreatePickablePeople(stage);
        CreatePickablePlatforms(stage);
    }

    private void CreateBarrels(int stage)
    {
        const float height = .5f;
        for (var i = 0; i < barrelNumbers / stage; i++)
        {
            Vector3 posToSpawn = GetRandomSpawnPosition() + Vector3.up * height;
            Instantiate(barrelsToSpawn, posToSpawn, Quaternion.identity);
        }
    }

    private void CreateGems(int stage)
    {
        if(_diamondsEnabled == false)
            return;
        const float height = .5f;
        for (var i = 0; i < gemsNumber / stage; i++)
        {
            Vector3 posToSpawn = GetRandomSpawnPosition() + Vector3.up * height;
            Instantiate(gemsToSpawn, posToSpawn, Quaternion.identity);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        const int eligibleDistanceToCenter = 40;
        Vector3 posToSpawn = Vector3.zero;
        float distanceToCenter = 0;
        while (distanceToCenter <= eligibleDistanceToCenter)
        {
            posToSpawn = new Vector3
            {
                x = Random.Range(0f, 1f) > 0.5f ? Random.Range(xBorderMin, xBorderMax) : 
                    Random.Range(-xBorderMax, -xBorderMin), 
                z = Random.Range(0f, 1f) > 0.5f ? Random.Range(yBorderMin, yBorderMax) : 
                    Random.Range(-yBorderMax, -yBorderMin)
            };
            distanceToCenter = Vector3.Distance(Vector3.zero, posToSpawn);
        }
        return posToSpawn;
    }

    private void CreateCoinChests(int stage)
    {
        const float height = .5f;
        for (var i = 0; i < coinsNumber / stage; i++)
        {
            while (true)
            {
                Vector3 posToSpawn = Vector3.up * height;
                posToSpawn.x = Random.Range(0f, 1f) > 0.5f ? Random.Range(xBorderMin, xBorderMax) : Random.Range(-xBorderMax, -xBorderMin);
                posToSpawn.z = Random.Range(0f, 1f) > 0.5f ? Random.Range(yBorderMin, yBorderMax) : Random.Range(-yBorderMax, -yBorderMin);

                if (IsOverlappingOtherGenerated(posToSpawn)) continue;
                Instantiate(coinsToSpawn, posToSpawn, Quaternion.identity);
                break;
            }
        }
    }

    private static bool IsOverlappingOtherGenerated(Vector3 posToSpawn)
    {
        var outCols = Physics.OverlapSphere(posToSpawn, 1, LayerMask.GetMask("Default"));
        bool isOverlapping = outCols != null && outCols.Length != 0;
        return isOverlapping;
    }

    private void CreatePickablePeople(int stage)
    {
        for (var i = 0; i < peopleNumber[stage - 1]; i++)
        {
            while (true)
            {
                Vector3 posToSpawn = GetRandomSpawnPosition();

                var outCols = Physics.OverlapSphere(posToSpawn, 2);
                if (outCols == null || outCols.Length == 0)
                {
                    Instantiate(peopleToSpawn[Random.Range(0, peopleToSpawn.Length)], posToSpawn, Quaternion.identity);
                    break;
                }
            }
        }
    }

    private void CreatePickablePlatforms(int stage)
    {
        for (var i = 0; i < platformsNumber[stage - 1]; i++)
        {
            
            Vector3 posToSpawn = GetRandomSpawnPosition();
            Instantiate(platformsToSpawn[Random.Range(0, platformsToSpawn.Length)], posToSpawn,
                Quaternion.identity);
            break;
        }
    }

    private List<Platform> ComeUpWithPeopleAndPlatformsCount(int stage, out List<People> people, out List<PeopleThatCanBeTaken> pickablePeople, out List<AttachablePlatform> pickablePlatforms)
    {
        int platInd;
        int peopInd;
        var platforms = new List<Platform>();
        people = new List<People>();
        pickablePeople = new List<PeopleThatCanBeTaken>();
        pickablePlatforms = new List<AttachablePlatform>();
        switch (stage)
        {
            case 1:
            {
                for (var j = 0; j < enemyPlatformsNumber[stage - 1]; j++)
                {
                    platInd = Random.Range(0, enemiesToSpawn1.Length);
                    pickablePlatforms.Add(enemiesToSpawn1Add[platInd]);
                    platforms.Add(enemiesToSpawn1[platInd]);
                }

                for (var j = 0; j < enemyPeopleNumber[stage - 1]; j++)
                {
                    peopInd = Random.Range(0, enemyPeopleToSpawn.Length);
                    people.Add(enemyPeopleToSpawn[peopInd]);
                    pickablePeople.Add(enemyPeopleToSpawnAdd[peopInd]);
                }

                break;
            }
            case 2:
            {
                for (var j = 0; j < enemyPlatformsNumber[stage - 1]; j++)
                {
                    platInd = Random.Range(0, enemiesToSpawn2.Length);
                    pickablePlatforms.Add(enemiesToSpawn2Add[platInd]);
                    platforms.Add(enemiesToSpawn2[platInd]);
                }

                for (var j = 0; j < enemyPeopleNumber[stage - 1]; j++)
                {
                    peopInd = Random.Range(0, enemyPeopleToSpawn.Length);
                    people.Add(enemyPeopleToSpawn[peopInd]);
                    pickablePeople.Add(enemyPeopleToSpawnAdd[peopInd]);
                }

                break;
            }
            case 3:
            {
                for (var j = 0; j < enemyPlatformsNumber[stage - 1]; j++)
                {
                    platInd = Random.Range(0, enemiesToSpawn2.Length);
                    pickablePlatforms.Add(enemiesToSpawn2Add[platInd]);
                    platforms.Add(enemiesToSpawn2[platInd]);
                }

                for (var j = 0; j < enemyPeopleNumber[stage - 1]; j++)
                {
                    peopInd = Random.Range(0, enemyPeopleToSpawn.Length);
                    people.Add(enemyPeopleToSpawn[peopInd]);
                    pickablePeople.Add(enemyPeopleToSpawnAdd[peopInd]);
                }

                break;
            }
            case 4:
            {
                for (var j = 0; j < enemyPlatformsNumber[stage - 1]; j++)
                {
                    platInd = Random.Range(0, enemiesToSpawn2.Length);
                    pickablePlatforms.Add(enemiesToSpawn2Add[platInd]);
                    platforms.Add(enemiesToSpawn2[platInd]);
                }

                for (var j = 0; j < enemyPeopleNumber[stage - 1]; j++)
                {
                    peopInd = Random.Range(0, enemyPeopleToSpawn.Length);
                    people.Add(enemyPeopleToSpawn[peopInd]);
                    pickablePeople.Add(enemyPeopleToSpawnAdd[peopInd]);
                }

                break;
            }
            default:
            {
                for (var j = 0; j < enemyPlatformsNumber[stage - 1]; j++)
                {
                    platInd = Random.Range(0, enemiesToSpawn2.Length);
                    pickablePlatforms.Add(enemiesToSpawn2Add[platInd]);
                    platforms.Add(enemiesToSpawn2[platInd]);
                }

                for (var j = 0; j < enemyPeopleNumber[stage - 1]; j++)
                {
                    peopInd = Random.Range(0, enemyPeopleToSpawn.Length);
                    people.Add(enemyPeopleToSpawn[peopInd]);
                    pickablePeople.Add(enemyPeopleToSpawnAdd[peopInd]);
                }

                break;
            }
        }

        return platforms;
    }


    public void ActuallyGenerateBoss()
    {
        Vector3 posToSpawn = new Vector3();
    
        while (true)
        {
            if (Random.Range(0f, 1f) > 0.5f)
                posToSpawn.x = Random.Range(xBorderMin, xBorderMax - 30);
            else
                posToSpawn.x = Random.Range(-xBorderMax + 30, -xBorderMin);
            if (Random.Range(0f, 1f) > 0.5f)
                posToSpawn.z = Random.Range(yBorderMin, yBorderMax - 30);
            else
                posToSpawn.z = Random.Range(-yBorderMax + 30, -yBorderMin);

            var intersections = Physics.SphereCastAll(posToSpawn, 30, Vector3.up);
            if(intersections.Any(x => x.transform.TryGetComponent<Platform>(out var platform) && platform.isEnemy == false))
            {
                continue;
            }

            Enemy enemy = Instantiate(bosses[stage - 1], posToSpawn, Quaternion.identity);
            List<Platform> platforms = new List<Platform>();
            List<People> people = new List<People>();
            List<PeopleThatCanBeTaken> peopleAdditive = new List<PeopleThatCanBeTaken>();
            List<AttachablePlatform> platformAdditive = new List<AttachablePlatform>();
            var platInd = 0;
            var peopInd = 0;
            if (stage == 1)
            {
                for (int j = 0; j < enemyPlatformsNumber[stage - 1] + 1; j++)
                {
                    platInd = Random.Range(0, enemiesToSpawn1.Length);
                    platformAdditive.Add(enemiesToSpawn1Add[platInd]);
                    platforms.Add(enemiesToSpawn1[platInd]);
                }
                for (int j = 0; j < enemyPeopleNumber[stage - 1] + 1; j++)
                {
                    peopInd = Random.Range(0, enemyPeopleToSpawn.Length);
                    people.Add(enemyPeopleToSpawn[peopInd]);
                    peopleAdditive.Add(enemyPeopleToSpawnAdd[peopInd]);
                }
            }
            else if (stage == 2)
            {
                for (int j = 0; j < enemyPlatformsNumber[stage - 1] + 1; j++)
                {
                    platInd = Random.Range(0, enemiesToSpawn2.Length);
                    platformAdditive.Add(enemiesToSpawn2Add[platInd]);
                    platforms.Add(enemiesToSpawn2[platInd]);
                }
                for (int j = 0; j < enemyPeopleNumber[stage - 1] + 1; j++)
                {
                    peopInd = Random.Range(0, enemyPeopleToSpawn.Length);
                    people.Add(enemyPeopleToSpawn[peopInd]);
                    peopleAdditive.Add(enemyPeopleToSpawnAdd[peopInd]);
                }
            }
            else if (stage == 3)
            {
                for (int j = 0; j < enemyPlatformsNumber[stage - 1] + 1; j++)
                {
                    platInd = Random.Range(0, enemiesToSpawn2.Length);
                    platformAdditive.Add(enemiesToSpawn2Add[platInd]);
                    platforms.Add(enemiesToSpawn2[platInd]);
                }
                for (int j = 0; j < enemyPeopleNumber[stage - 1] + 1; j++)
                {
                    peopInd = Random.Range(0, enemyPeopleToSpawn.Length);
                    people.Add(enemyPeopleToSpawn[peopInd]);
                    peopleAdditive.Add(enemyPeopleToSpawnAdd[peopInd]);
                }
            }
            else if (stage == 4)
            {
                for (int j = 0; j < enemyPlatformsNumber[stage - 1] + 1; j++)
                {
                    platInd = Random.Range(0, enemiesToSpawn2.Length);
                    platformAdditive.Add(enemiesToSpawn2Add[platInd]);
                    platforms.Add(enemiesToSpawn2[platInd]);
                }
                for (int j = 0; j < enemyPeopleNumber[stage - 1] + 1; j++)
                {
                    peopInd = Random.Range(0, enemyPeopleToSpawn.Length);
                    people.Add(enemyPeopleToSpawn[peopInd]);
                    peopleAdditive.Add(enemyPeopleToSpawnAdd[peopInd]);
                }
            }
            else
            {
                for (int j = 0; j < enemyPlatformsNumber[stage - 1] + 1; j++)
                {
                    platInd = Random.Range(0, enemiesToSpawn2.Length);
                    platformAdditive.Add(enemiesToSpawn2Add[platInd]);
                    platforms.Add(enemiesToSpawn2[platInd]);
                }
                for (int j = 0; j < enemyPeopleNumber[stage - 1] + 1; j++)
                {
                    peopInd = Random.Range(0, enemyPeopleToSpawn.Length);
                    people.Add(enemyPeopleToSpawn[peopInd]);
                    peopleAdditive.Add(enemyPeopleToSpawnAdd[peopInd]);
                }
            }

            if(enemy.boss5Stage)
            {
                enemy.Material = _materials.GetMaterialForBoss5Stage();
            }
            else 
            {
                enemy.Material = _materials.GetRandom();
            }
            enemy.SpawnEnvironment(platforms.ToArray(), people.ToArray(), hpIncrease[stage - 1] + 1, damageIncrease[stage - 1] + 1, platformAdditive, peopleAdditive);
            GameManager.instance.boss = enemy;
            BossCreated?.Invoke(enemy);
            enemy.Died += Game.GameManager.BossDied;
            break;
        }
    }
    
    public void GenerateBoss()
    {
        _appearing.QueryBossSpawn(ActuallyGenerateBoss);
    }

    private void OnDrawGizmos()
    {
        const int height = 3;
        Gizmos.DrawLine(new Vector3(-xBorderMin, height, -yBorderMin), new Vector3(-xBorderMin, height, yBorderMax));
        Gizmos.DrawLine(new Vector3(-xBorderMin, height, yBorderMax), new Vector3(-xBorderMax, height, -yBorderMin));
        Gizmos.DrawLine(new Vector3(xBorderMax, height, -yBorderMin), new Vector3(xBorderMax, height, yBorderMax));
        Gizmos.DrawLine(new Vector3(xBorderMax, height, yBorderMax), new Vector3(-xBorderMin, height, -yBorderMin));
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DefaultNamespace.Common;
using Infrastructure;
using InputSystem;
using RaftWars.Infrastructure;
using RaftWars.Infrastructure.Services;
using RaftWars.Pickables;
using SpecialPlatforms;
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
    private int _counter;
    private int _offscreenGenerationCooldown;
    private IEnumerable<(Pickable loaded, SpecialPlatform meta)> _ownedPickableSpecialPlatforms;
    private IEnumerable<Platform> _ownedReadySpecialPlatforms;
    private AttachablePlatform _emptyPlatform;
    private int _guranteedEmptyPlatforms = 3;

    public void Construct(BossAppearing appearing, IEnumerable<(Pickable loaded, SpecialPlatform meta)> ownedPickable)
    {
        _emptyPlatform = platformsToSpawn.First();
        _appearing = appearing;
        _diamondsEnabled = Game.FeatureFlags.DiamondsEnabledInGame;
        _collectibles = Game.CollectiblesService;
        _materials = Game.MaterialsService;
        _player = Game.PlayerService;
        SetOwnedPickables(ownedPickable);
    }

    public void SetOwnedPickables(IEnumerable<(Pickable loaded, SpecialPlatform meta)> ownedPickable)
    {
        _ownedPickableSpecialPlatforms = ownedPickable;
        _ownedReadySpecialPlatforms = ownedPickable.Select(x => x.loaded)
            .Cast<AttachablePlatform>().Select(x => x.platform.GetComponent<Platform>());
        Filter();
    }

    private void Filter()
    {
        platformsToSpawn = _ownedPickableSpecialPlatforms.Select(x => x.loaded)
            .Cast<AttachablePlatform>()
            .Append(_emptyPlatform)
            .ToArray();
        enemiesToSpawn1 = FilterReadyPlatforms(enemiesToSpawn1).ToArray();
        enemiesToSpawn2 = FilterReadyPlatforms(enemiesToSpawn2).ToArray();
        enemiesToSpawn3 = FilterReadyPlatforms(enemiesToSpawn3).ToArray();
        enemiesToSpawn4 = FilterReadyPlatforms(enemiesToSpawn4).ToArray();
        enemiesToSpawn5 = FilterReadyPlatforms(enemiesToSpawn5).ToArray();
        enemiesToSpawn1Add = FilterAddPlatforms(enemiesToSpawn1Add).Cast<AttachablePlatform>().ToArray();
        enemiesToSpawn2Add = FilterAddPlatforms(enemiesToSpawn2Add).Cast<AttachablePlatform>().ToArray();
        enemiesToSpawn3Add = FilterAddPlatforms(enemiesToSpawn3Add).Cast<AttachablePlatform>().ToArray();
        enemiesToSpawn4Add = FilterAddPlatforms(enemiesToSpawn4Add).Cast<AttachablePlatform>().ToArray();
        enemiesToSpawn5Add = FilterAddPlatforms(enemiesToSpawn5Add).Cast<AttachablePlatform>().ToArray();
    }

    private IEnumerable<Platform> FilterReadyPlatforms(IEnumerable<Platform> platforms)
    {
        foreach (Platform platform in platforms)
        {
            if (platform.emptyPlatform)
            {
                yield return platform;
                continue;
            }
            yield return _ownedReadySpecialPlatforms.Random();
        }
    }

    private IEnumerable<Pickable> FilterAddPlatforms(IEnumerable<Pickable> platforms)
    {
        foreach (Pickable platform in platforms)
        {
            if (platform.notExcludable)
            {
                yield return platform;
                continue;
            }
            yield return _ownedPickableSpecialPlatforms.Random().loaded;
        }
    }

    private void Start()
    {
        if(Game.GameManager == null)
            return;
        Game.GameManager.GameStarted += () => StartCoroutine(Counter());
    }

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
            
            const float heightOverTheCollider = 4.5f;
            if (TryHitWater(out RaycastHit hit))
            {
                Debug.Log($"Spawned collectable on way");
                Vector3 spawnPoint = hit.point + hit.normal * heightOverTheCollider;
                var pickables = WhichOneToSpawnProbabilityCheck(ref probabilityToSpawnPlatformInPercent, 
                    out GeneratedType generatedType);

                Pickable prefabToSpawn = pickables.ElementAt(Random.Range(0, pickables.Count()));
                Pickable collectible = Instantiate(prefabToSpawn, spawnPoint, Quaternion.identity);
                if (CanCreateRewardedSpecialPlatform(collectible, generatedType, out AttachablePlatform platform))
                {
                    var platformMeta = _ownedPickableSpecialPlatforms.First(x => x.loaded == prefabToSpawn).meta;
                    GameFactory.CreateRaftPieceAdvertising(platform, platformMeta.LocalizedName);
                }

                collectible.name = "OffScreenGeneratedOverTime";
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

    private static bool CanCreateRewardedSpecialPlatform(Pickable pickable, GeneratedType generatedType, out AttachablePlatform platform) 
        => pickable.TryGetComponent(out platform) && 
           generatedType == GeneratedType.SpecialPlatform && 
           RandomExtension.ProbabilityCheck(.5f);

    private IEnumerable<Pickable> WhichOneToSpawnProbabilityCheck(ref int probabilityToSpawnPlatformInPercent, out GeneratedType generatedType)
    {
        #region Debug

        const bool AlwaysSpecialPlatforms = false;
        if (AlwaysSpecialPlatforms)
        {
            generatedType = GeneratedType.SpecialPlatform;
            return _ownedPickableSpecialPlatforms.Select(x => x.loaded);
        }

        #endregion

        if (Random.Range(0, 100) > ProbabilityToSpawnPlatformInPercent(probabilityToSpawnPlatformInPercent))
        {
            probabilityToSpawnPlatformInPercent = 0;
            if (_guranteedEmptyPlatforms > 0)
            {
                _guranteedEmptyPlatforms--;
                generatedType = GeneratedType.EmptyPlatform;
                return Enumerable.Repeat(_emptyPlatform, 1);
            }

            generatedType = GeneratedType.SpecialPlatform;
            return _ownedPickableSpecialPlatforms.Select(x => x.loaded);
        }
        probabilityToSpawnPlatformInPercent += 10;
        generatedType = GeneratedType.People;
        return peopleToSpawn;
    }

    enum GeneratedType
    {
        EmptyPlatform,
        SpecialPlatform,
        People
    }

    private static float ProbabilityToSpawnPlatformInPercent(int probabilityToSpawnPlatformInPercent)
    {
        return 80f - probabilityToSpawnPlatformInPercent;
    }

    private bool TryHitWater(out RaycastHit hit)
    { 
        Vector3 direction = _player.MoveDirectionXZ;
        direction.y = direction.z;;
        var center = new Vector2(.5f, .5f);
        const float generationViewportDistance = 2;
        Vector2 viewportPoint = direction.normalized * generationViewportDistance * center + center;
        Ray ray = Camera.main.ViewportPointToRay(viewportPoint);
        int raycastPlane = LayerMask.GetMask("Water");
        bool isHit = Physics.Raycast(ray, out hit, Mathf.Infinity, raycastPlane);
        return isHit;
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
            const int checkRadius = 40;
            var intersections = Physics.OverlapSphere(posToSpawn, checkRadius);
            if(intersections.Any(x => x.transform.TryGetComponent<Platform>(out var platform) && platform.isEnemy == false))
                continue;
            Enemy enemy = Instantiate(enemyPrefab, posToSpawn, Quaternion.identity);
            enemy.Construct(AllServices.GetSingle<IEnumerable<SpecialPlatform>>());
            var platforms = ComeUpWithPeopleAndPlatformsCount(stage, out var people, 
                out var pickablePeople, out var pickablePlatforms);
            enemy.Material = _materials.GetRandom();
            enemy.SpawnEnvironment(platforms.ToArray(), people.ToArray(), hpIncrease[stage - 1],
                damageIncrease[stage - 1], pickablePlatforms, pickablePeople);
            GameManager.instance.AddEnemy(enemy);
            i++;
        }
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

    public Vector3 GetRandomSpawnPosition()
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

            const float checkRadius = 40;
            var intersections = Physics.OverlapSphere(posToSpawn, checkRadius);
            if(intersections.Any(x => x.transform.TryGetComponent<Platform>(out var platform) && platform.isEnemy == false))
            {
                continue;
            }

            Enemy enemy = Instantiate(bosses[stage - 1], posToSpawn, Quaternion.identity);
            enemy.Construct(AllServices.GetSingle<IEnumerable<SpecialPlatform>>());
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
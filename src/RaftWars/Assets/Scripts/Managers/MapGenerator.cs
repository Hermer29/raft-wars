using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private Enemy enemy;
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

    private int stage = 1;

    public void Generate(int stage)
    {
        this.stage = stage;
        FigureOutPlatformsAndEnemies(stage); 
        CreateActivePlatforms(stage);
        CreateActivePeople(stage);
        CreateCoinChests(stage);
        CreateGems(stage);
        CreateBarrels(stage);
    }

    private void FigureOutPlatformsAndEnemies(int stage)
    {
        for (var i = 0; i < enemiesNumber[stage - 1]; i++)
        {
            while (true)
            {
                Vector3 posToSpawn = GetRandomSpawnPosition();

                var outCols = Physics.OverlapSphere(posToSpawn, 5);
                if (outCols != null && outCols.Length != 0) continue;
                Enemy _enemy = Instantiate(enemy, posToSpawn, Quaternion.identity);
                var platforms = ComeUpWithEnvironment(stage, out var people, out var pickablePeople
                    , out var pickablePlatforms);
                _enemy.SpawnEnvironment(platforms.ToArray(), people.ToArray(), hpIncrease[stage - 1],
                    damageIncrease[stage - 1], pickablePlatforms, pickablePeople);
                GameManager.instance.AddEnemy(_enemy);
                break;
            }
        }
    }

    public void SpawnMiscellaneous()
    {
        CreateBarrels(stage);
        CreateGems(stage);
        CreateCoinChests(stage);
    }

    private void CreateBarrels(int stage)
    {
        const float height = .5f;
        for (var i = 0; i < barrelNumbers / stage; i++)
        {
            while (true)
            {
                Vector3 posToSpawn = GetRandomSpawnPosition() + Vector3.up * height;

                var outCols = Physics.OverlapSphere(posToSpawn, 1);
                if (outCols == null || outCols.Length == 0)
                {
                    Instantiate(barrelsToSpawn, posToSpawn, Quaternion.identity);
                    break;
                }
            }
        }
    }

    private void CreateGems(int stage)
    {
        const float height = .5f;
        for (var i = 0; i < gemsNumber / stage; i++)
        {
            while (true)
            {
                Vector3 posToSpawn = GetRandomSpawnPosition() + Vector3.up * height;

                var outCols = Physics.OverlapSphere(posToSpawn, 1);
                if (outCols == null || outCols.Length == 0)
                {
                    Instantiate(gemsToSpawn, posToSpawn, Quaternion.identity);
                    break;
                }
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        var posToSpawn = new Vector3
        {
            x = Random.Range(0f, 1f) > 0.5f ? 
                Random.Range(xBorderMin, xBorderMax) : 
                Random.Range(-xBorderMax, -xBorderMin),
            z = Random.Range(0f, 1f) > 0.5f ? 
                Random.Range(yBorderMin, yBorderMax) : 
                Random.Range(-yBorderMax, -yBorderMin)
        };
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

                var outCols = Physics.OverlapSphere(posToSpawn, 1);
                if (outCols != null && outCols.Length != 0) continue;
                Instantiate(coinsToSpawn, posToSpawn, Quaternion.identity);
                break;
            }
        }
    }

    private void CreateActivePeople(int stage)
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

    private void CreateActivePlatforms(int stage)
    {
        for (var i = 0; i < platformsNumber[stage - 1]; i++)
        {
            while (true)
            {
                Vector3 posToSpawn = GetRandomSpawnPosition();

                var outCols = Physics.OverlapSphere(posToSpawn, 2);
                if (outCols == null || outCols.Length == 0)
                {
                    Instantiate(platformsToSpawn[Random.Range(0, platformsToSpawn.Length)], posToSpawn,
                        Quaternion.identity);
                    break;
                }
            }
        }
    }

    private List<Platform> ComeUpWithEnvironment(int stage, out List<People> people, out List<PeopleThatCanBeTaken> pickablePeople, out List<AttachablePlatform> pickablePlatforms)
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


    public void GenerateBoss()
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

            var outCols = Physics.OverlapSphere(posToSpawn, 3);
            if (outCols == null || outCols.Length == 0)
            {
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
                enemy.SpawnEnvironment(platforms.ToArray(), people.ToArray(), hpIncrease[stage - 1] + 1, damageIncrease[stage - 1] + 1, platformAdditive, peopleAdditive);
                GameManager.instance.boss = enemy;
                break;
            }
        }
    }
}

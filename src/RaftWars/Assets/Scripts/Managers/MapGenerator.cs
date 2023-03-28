using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private Enemy enemy;
    [SerializeField] private Platform[] enemiesToSpawn1;
    [SerializeField] private PlatformAdditive[] enemiesToSpawn1Add;
    [SerializeField] private Platform[] enemiesToSpawn2;
    [SerializeField] private PlatformAdditive[] enemiesToSpawn2Add;
    [SerializeField] private Platform[] enemiesToSpawn3;
    [SerializeField] private PlatformAdditive[] enemiesToSpawn3Add;
    [SerializeField] private Platform[] enemiesToSpawn4;
    [SerializeField] private PlatformAdditive[] enemiesToSpawn4Add;
    [SerializeField] private Platform[] enemiesToSpawn5;
    [SerializeField] private PlatformAdditive[] enemiesToSpawn5Add;
    [SerializeField] private int[] enemiesNumber;
    [SerializeField] private int[] enemyPeopleNumber;
    [SerializeField] private int[] hpIncrease;
    [SerializeField] private int[] damageIncrease;
    [SerializeField] private int[] enemyPlatformsNumber;
    [SerializeField] private People[] enemyPeopleToSpawn;
    [SerializeField] private PeopleAdditive[] enemyPeopleToSpawnAdd;

    [SerializeField] private Enemy[] bosses;

    [Header("Platforms")]
    [SerializeField] private PlatformAdditive[] platformsToSpawn;
    [SerializeField] private int[] platformsNumber;
    
    [Header("People")]
    [SerializeField] private PeopleAdditive[] peopleToSpawn;
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
        Collider[] outCols;
        Vector3 posToSpawn = new Vector3();
        int platInd = 0;
        int peopInd = 0;
        for(int i = 0; i < enemiesNumber[stage - 1]; i++)
        {
            while (true)
            {
                if (Random.Range(0f, 1f) > 0.5f)
                    posToSpawn.x = Random.Range(xBorderMin, xBorderMax);
                else
                    posToSpawn.x = Random.Range(-xBorderMax, -xBorderMin);
                if (Random.Range(0f, 1f) > 0.5f)
                    posToSpawn.z = Random.Range(yBorderMin, yBorderMax);
                else
                    posToSpawn.z = Random.Range(-yBorderMax, -yBorderMin);

                outCols = Physics.OverlapSphere(posToSpawn, 5);
                Enemy _enemy;
                if (outCols == null || outCols.Length == 0)
                {
                    _enemy = Instantiate(enemy, posToSpawn, Quaternion.identity);
                    List<Platform> platforms = new List<Platform>();
                    List<People> people = new List<People>();
                    List<PeopleAdditive> peopleAdditive = new List<PeopleAdditive>();
                    List<PlatformAdditive> platformAdditive = new List<PlatformAdditive>();
                    if (stage == 1)
                    {
                        for(int j = 0; j < enemyPlatformsNumber[stage - 1]; j++)
                        {
                            platInd = Random.Range(0, enemiesToSpawn1.Length);
                            platformAdditive.Add(enemiesToSpawn1Add[platInd]);
                            platforms.Add(enemiesToSpawn1[platInd]);
                        }
                        for(int j = 0; j < enemyPeopleNumber[stage - 1]; j++)
                        {
                            peopInd = Random.Range(0, enemyPeopleToSpawn.Length);
                            people.Add(enemyPeopleToSpawn[peopInd]);
                            peopleAdditive.Add(enemyPeopleToSpawnAdd[peopInd]);
                        }
                    }
                    else if(stage == 2)
                    {
                        for (int j = 0; j < enemyPlatformsNumber[stage - 1]; j++)
                        {
                            platInd = Random.Range(0, enemiesToSpawn2.Length);
                            platformAdditive.Add(enemiesToSpawn2Add[platInd]);
                            platforms.Add(enemiesToSpawn2[platInd]);
                        }
                        for (int j = 0; j < enemyPeopleNumber[stage - 1]; j++)
                        {
                            peopInd = Random.Range(0, enemyPeopleToSpawn.Length);
                            people.Add(enemyPeopleToSpawn[peopInd]);
                            peopleAdditive.Add(enemyPeopleToSpawnAdd[peopInd]);
                        }
                    }
                    else if (stage == 3)
                    {
                        for (int j = 0; j < enemyPlatformsNumber[stage - 1]; j++)
                        {
                            platInd = Random.Range(0, enemiesToSpawn2.Length);
                            platformAdditive.Add(enemiesToSpawn2Add[platInd]);
                            platforms.Add(enemiesToSpawn2[platInd]);
                        }
                        for (int j = 0; j < enemyPeopleNumber[stage - 1]; j++)
                        {
                            peopInd = Random.Range(0, enemyPeopleToSpawn.Length);
                            people.Add(enemyPeopleToSpawn[peopInd]);
                            peopleAdditive.Add(enemyPeopleToSpawnAdd[peopInd]);
                        }
                    }
                    else if (stage == 4)
                    {
                        for (int j = 0; j < enemyPlatformsNumber[stage - 1]; j++)
                        {
                            platInd = Random.Range(0, enemiesToSpawn2.Length);
                            platformAdditive.Add(enemiesToSpawn2Add[platInd]);
                            platforms.Add(enemiesToSpawn2[platInd]);
                        }
                        for (int j = 0; j < enemyPeopleNumber[stage - 1]; j++)
                        {
                            peopInd = Random.Range(0, enemyPeopleToSpawn.Length);
                            people.Add(enemyPeopleToSpawn[peopInd]);
                            peopleAdditive.Add(enemyPeopleToSpawnAdd[peopInd]);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < enemyPlatformsNumber[stage - 1]; j++)
                        {
                            platInd = Random.Range(0, enemiesToSpawn2.Length);
                            platformAdditive.Add(enemiesToSpawn2Add[platInd]);
                            platforms.Add(enemiesToSpawn2[platInd]);
                        }
                        for (int j = 0; j < enemyPeopleNumber[stage - 1]; j++)
                        {
                            peopInd = Random.Range(0, enemyPeopleToSpawn.Length);
                            people.Add(enemyPeopleToSpawn[peopInd]);
                            peopleAdditive.Add(enemyPeopleToSpawnAdd[peopInd]);
                        }
                    }
                    _enemy.Spawn(platforms.ToArray(), people.ToArray(), hpIncrease[stage - 1], damageIncrease[stage - 1], platformAdditive, peopleAdditive);
                    GameManager.instance.AddEnemy(_enemy);
                    break;
                }
            }
        }



        for (int i = 0; i < platformsNumber[stage - 1]; i++)
        {
            while (true)
            {
                if (Random.Range(0f, 1f) > 0.5f)
                    posToSpawn.x = Random.Range(xBorderMin, xBorderMax);
                else
                    posToSpawn.x = Random.Range(-xBorderMax, -xBorderMin);
                if (Random.Range(0f, 1f) > 0.5f)
                    posToSpawn.z = Random.Range(yBorderMin, yBorderMax);
                else
                    posToSpawn.z = Random.Range(-yBorderMax, -yBorderMin);

                outCols = Physics.OverlapSphere(posToSpawn, 2);
                if (outCols == null || outCols.Length == 0)
                {
                    Instantiate(platformsToSpawn[Random.Range(0, platformsToSpawn.Length)], posToSpawn, Quaternion.identity);
                    break;
                }
            }
        }


        for (int i = 0; i < peopleNumber[stage - 1]; i++)
        {
            while (true)
            {
                if (Random.Range(0f, 1f) > 0.5f)
                    posToSpawn.x = Random.Range(xBorderMin, xBorderMax);
                else
                    posToSpawn.x = Random.Range(-xBorderMax, -xBorderMin);
                if (Random.Range(0f, 1f) > 0.5f)
                    posToSpawn.z = Random.Range(yBorderMin, yBorderMax);
                else
                    posToSpawn.z = Random.Range(-yBorderMax, -yBorderMin);

                outCols = Physics.OverlapSphere(posToSpawn, 2);
                if (outCols == null || outCols.Length == 0)
                {
                    Instantiate(peopleToSpawn[Random.Range(0, peopleToSpawn.Length)], posToSpawn, Quaternion.identity);
                    break;
                }
            }
        }

        posToSpawn.y += 0.5f;
        for (int i = 0; i < coinsNumber / stage; i++)
        {
            while (true)
            {
                if (Random.Range(0f, 1f) > 0.5f)
                    posToSpawn.x = Random.Range(xBorderMin, xBorderMax);
                else
                    posToSpawn.x = Random.Range(-xBorderMax, -xBorderMin);
                if (Random.Range(0f, 1f) > 0.5f)
                    posToSpawn.z = Random.Range(yBorderMin, yBorderMax);
                else
                    posToSpawn.z = Random.Range(-yBorderMax, -yBorderMin);

                outCols = Physics.OverlapSphere(posToSpawn, 1);
                if (outCols == null || outCols.Length == 0)
                {
                    Instantiate(coinsToSpawn, posToSpawn, Quaternion.identity);
                    break;
                }
            }
        }

        for (int i = 0; i < gemsNumber / stage; i++)
        {
            while (true)
            {
                if (Random.Range(0f, 1f) > 0.5f)
                    posToSpawn.x = Random.Range(xBorderMin, xBorderMax);
                else
                    posToSpawn.x = Random.Range(-xBorderMax, -xBorderMin);
                if (Random.Range(0f, 1f) > 0.5f)
                    posToSpawn.z = Random.Range(yBorderMin, yBorderMax);
                else
                    posToSpawn.z = Random.Range(-yBorderMax, -yBorderMin);

                outCols = Physics.OverlapSphere(posToSpawn, 1);
                if (outCols == null || outCols.Length == 0)
                {
                    Instantiate(gemsToSpawn, posToSpawn, Quaternion.identity);
                    break;
                }
            }
        }

        for(int i = 0; i < barrelNumbers / stage; i++)
        {
            while (true)
            {
                if (Random.Range(0f, 1f) > 0.5f)
                    posToSpawn.x = Random.Range(xBorderMin, xBorderMax);
                else
                    posToSpawn.x = Random.Range(-xBorderMax, -xBorderMin);
                if (Random.Range(0f, 1f) > 0.5f)
                    posToSpawn.z = Random.Range(yBorderMin, yBorderMax);
                else
                    posToSpawn.z = Random.Range(-yBorderMax, -yBorderMin);

                outCols = Physics.OverlapSphere(posToSpawn, 1);
                if (outCols == null || outCols.Length == 0)
                {
                    Instantiate(barrelsToSpawn, posToSpawn, Quaternion.identity);
                    break;
                }
            }
        }
    }


    public void GenerateBoss()
    {
        Collider[] outCols;
        Vector3 posToSpawn = new Vector3();
        int platInd = 0;
        int peopInd = 0;
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

            outCols = Physics.OverlapSphere(posToSpawn, 3);
            if (outCols == null || outCols.Length == 0)
            {
                Enemy enemy = Instantiate(bosses[stage - 1], posToSpawn, Quaternion.identity);
                List<Platform> platforms = new List<Platform>();
                List<People> people = new List<People>();
                List<PeopleAdditive> peopleAdditive = new List<PeopleAdditive>();
                List<PlatformAdditive> platformAdditive = new List<PlatformAdditive>();
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
                enemy.Spawn(platforms.ToArray(), people.ToArray(), hpIncrease[stage - 1] + 1, damageIncrease[stage - 1] + 1, platformAdditive, peopleAdditive);
                GameManager.instance.boss = enemy;
                break;
            }
        }
    }
}

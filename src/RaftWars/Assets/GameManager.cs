using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    private bool started = false;
    
    [Header("UI")]
    [SerializeField] private GameObject tapToPlay;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject failedPanel;
    [SerializeField] private GameObject blackBG;
    [SerializeField] private GameObject stagePanel;
    [SerializeField] private Image progressFill;
    [Space]
    [SerializeField] private Text damagePercentText, damageCostText, hpPercentText, hpCostText;
    [SerializeField] private Text progressText;
    [SerializeField] private Text platformsCountPrev, platformsCountAdd;
    [SerializeField] private Text warriorsCountPrev, warriorsCountAdd;
    [SerializeField] private Text powerCountPrev, powerCountAdd;
    [SerializeField] private Text healthCountPrev, healthCountAdd;

    private int platformsCountOnStart = 1;
    private int warriorsCountOnStart = 2;
    private int powerCountOnStart = 10;
    private int healthCountOnStart = 10;


    [Header("Managers")]
    [SerializeField] private MapGenerator map;
    private List<Enemy> enemies = new List<Enemy>();

    private int stage = 1;
    public Enemy boss;

    private double damagePercent = 0.2f, hpPercent = 0.2f;
    private int damageCost = 10, hpCost = 10;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(instance);
            instance = this;
        }

        map.Generate(stage);
        map.GenerateBoss();
    }


    private void Update()
    {
        if (!started)
        {
            if (Input.GetMouseButtonUp(0))
            {
                started = true;
                tapToPlay.SetActive(false);
                Player.instance.canPlay = true;
            }
        }
        else if(stage == 5 && boss == null && !stagePanel.activeSelf)
        {
            Player.instance.canPlay = false;
            blackBG.SetActive(true);
            winPanel.SetActive(true);
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
            if(PlayerPrefs.GetInt("Level") == 6)
            {
                PlayerPrefs.SetInt("Level", 1);
            }
        }
        else if(stage != 5 && boss == null && !stagePanel.activeSelf)
        {
            Player.instance.canPlay = false;
            blackBG.SetActive(true);
            platformsCountPrev.text = platformsCountOnStart.ToString();
            warriorsCountPrev.text = warriorsCountOnStart.ToString();
            powerCountPrev.text = powerCountOnStart.ToString();
            healthCountPrev.text = healthCountOnStart.ToString();

            platformsCountAdd.text = "+" + (Player.instance.platformCount - platformsCountOnStart).ToString();
            if(Player.instance.warriorsCount - warriorsCountOnStart >= 0)
                warriorsCountAdd.text = "+" + (Player.instance.warriorsCount - warriorsCountOnStart);
            else
                warriorsCountAdd.text = (Player.instance.warriorsCount - warriorsCountOnStart).ToString();
            
            if (Player.instance.fullDamage - powerCountOnStart >= 0)
                powerCountAdd.text = "+" + (Player.instance.fullDamage - powerCountOnStart);
            else
                powerCountAdd.text = (Player.instance.fullDamage - powerCountOnStart).ToString();
            
            if (Player.instance.fullHp - healthCountOnStart >= 0)
                healthCountAdd.text = "+" + (Player.instance.fullHp - healthCountOnStart);
            else
                healthCountAdd.text = (Player.instance.fullHp - healthCountOnStart).ToString();

            warriorsCountOnStart = Player.instance.warriorsCount;
            platformsCountOnStart = Player.instance.platformCount;
            powerCountOnStart = (int)Player.instance.fullDamage;
            healthCountOnStart = (int)Player.instance.fullHp;

            stagePanel.SetActive(true);
            progressFill.fillAmount = stage / 5f;
            stage++;
            Debug.Log(stage);
        }

        for(int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null)
                enemies.RemoveAt(i);
        }
    }
    
    public void AddEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void NextStage()
    {
        progressText.text = stage + "/5";
        map.Generate(stage);
        Player.instance.canPlay = true;
        blackBG.SetActive(false);
        stagePanel.SetActive(false);
        map.GenerateBoss();
    }

    public void Continue()
    {
        SceneManager.LoadScene("Level" + PlayerPrefs.GetInt("Level"));
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("Level" + PlayerPrefs.GetInt("Level", 1));
    }

    public void Failed()
    {
        blackBG.SetActive(true);
        failedPanel.SetActive(true);
    }

    public void IncreaseHealth()
    {
        if (Player.instance.coins >= hpCost)
        {
            Player.instance.AddCoins(-hpCost);
            hpCost += 10;
            Player.instance.AddHpBonus((float)hpPercent);
            hpPercent += 0.1f;
            hpPercent = Math.Round(hpPercent, 4);
            hpPercentText.text = "+" + hpPercent * 100 + "%";
            hpCostText.text = hpCost.ToString();
        }
    }

    public void IncreaseDamage()
    {
        if(Player.instance.coins >= damageCost)
        {
            Player.instance.AddCoins(-damageCost);
            damageCost += 10;
            Player.instance.AddDamageBonus((float)damagePercent);
            damagePercent += 0.1f;
            damagePercent = Math.Round(damagePercent, 4);
            damagePercentText.text = "+" + damagePercent * 100 + "%";
            damageCostText.text = damageCost.ToString();
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    private bool _started;
    
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
    private readonly List<Enemy> _enemies = new List<Enemy>();

    private int _stage = 1;
    public Enemy boss;

    private float buyableDamagePercent = 0.2f, buyableHealthPercent = 0.2f;
    private int damageCost = 10, healthAmplificationCost = 10;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
            instance = this;
        }

        map.Generate(_stage);
        map.GenerateBoss();
    }
    
    private void Update()
    {
        if (!_started)
        {
            StartGameOnClick();
        }
        else if(_stage == 5 && boss == null && !stagePanel.activeSelf)
        {
            WinTheGame();
        }
        else if(_stage != 5 && boss == null && !stagePanel.activeSelf)
        {
            WarmupStage();
        }

        for(var i = 0; i < _enemies.Count; i++)
        {
            if (_enemies[i] == null)
                _enemies.RemoveAt(i);
        }
    }

    private void WarmupStage()
    {
        Player.instance.canPlay = false;
        blackBG.SetActive(true);
        WarmupUiStats();
        WarmupStats();
        stagePanel.SetActive(true);
        progressFill.fillAmount = _stage / 5f;
        _stage++;
    }

    private void WarmupStats()
    {
        platformsCountAdd.text = "+" + (Player.instance.platformCount - platformsCountOnStart);
        if (Player.instance.warriorsCount - warriorsCountOnStart >= 0)
        {
            warriorsCountAdd.text = "+" + (Player.instance.warriorsCount - warriorsCountOnStart);
        }
        else
        {
            warriorsCountAdd.text = (Player.instance.warriorsCount - warriorsCountOnStart).ToString();
        }

        if (Player.instance.damage - powerCountOnStart >= 0)
        {
            powerCountAdd.text = "+" + (Player.instance.damage - powerCountOnStart);
        }
        else
        {
            powerCountAdd.text = (Player.instance.damage - powerCountOnStart).ToString();
        }

        if (Player.instance.hp - healthCountOnStart >= 0)
        {
            healthCountAdd.text = "+" + (Player.instance.hp - healthCountOnStart);
        }
        else
        {
            healthCountAdd.text = (Player.instance.hp - healthCountOnStart).ToString();
        }

        warriorsCountOnStart = Player.instance.warriorsCount;
        platformsCountOnStart = Player.instance.platformCount;
        powerCountOnStart = (int)Player.instance.damage;
        healthCountOnStart = (int)Player.instance.hp;
    }

    private void WarmupUiStats()
    {
        platformsCountPrev.text = platformsCountOnStart.ToString();
        warriorsCountPrev.text = warriorsCountOnStart.ToString();
        powerCountPrev.text = powerCountOnStart.ToString();
        healthCountPrev.text = healthCountOnStart.ToString();
    }

    private void WinTheGame()
    {
        Player.instance.canPlay = false;
        blackBG.SetActive(true);
        winPanel.SetActive(true);
        IncrementLevel();
    }

    private static void IncrementLevel()
    {
        void Increment()
        {
            PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
        }

        //Increment();
        LoopLevelsOnOverflow();
    }

    private static void LoopLevelsOnOverflow()
    {
        if (PlayerPrefs.GetInt("Level") == 6)
        {
            PlayerPrefs.SetInt("Level", 1);
        }
    }

    private void StartGameOnClick()
    {
        if (!Input.GetMouseButtonUp(0)) return;
        _started = true;
        tapToPlay.SetActive(false);
        Player.instance.canPlay = true;
    }

    public void AddEnemy(Enemy enemy)
    {
        _enemies.Add(enemy);
    }

    public void NextStage()
    {
        progressText.text = _stage + "/5";
        map.Generate(_stage);
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

    public void PlayerLost()
    {
        blackBG.SetActive(true);
        failedPanel.SetActive(true);
    }

    public void IncreaseHealth()
    {
        bool IsNotEnoughMoneyToIncreaseHealth()
        {
            return Player.instance.coins < healthAmplificationCost;
        }

        if (IsNotEnoughMoneyToIncreaseHealth()) return;

        Player.instance.SpendCoins(healthAmplificationCost);
        Player.instance.IncreaseHealth((float)buyableHealthPercent);
        UpdateNextHealthUpgrade();
    }

    private void UpdateNextHealthUpgrade()
    {
        void MakeNextHealthUpgradeMorePowerful()
        {
            buyableHealthPercent += 0.1f;
            var integerPercent = (int)(buyableHealthPercent * 100);
            hpPercentText.text = $"+{integerPercent}%";
        }

        void IncreaseHealthUpgradeCost()
        {
            healthAmplificationCost += 10;
            hpCostText.text = healthAmplificationCost.ToString();
        }
        
        MakeNextHealthUpgradeMorePowerful();
        IncreaseHealthUpgradeCost();
    }
    
    public void IncreaseDamage()
    {
        bool IsNotEnoughCoinsToAmplifyDamage()
        {
            return Player.instance.coins < damageCost;
        }

        if (IsNotEnoughCoinsToAmplifyDamage()) return;
        
        Player.instance.SpendCoins(damageCost);
        Player.instance.AmplifyDamage(buyableDamagePercent);
        UpdateNextDamageUpgrade();
    }

    private void UpdateNextDamageUpgrade()
    {
        void IncreaseNextDamageAmplification()
        {
            buyableDamagePercent += 0.1f;
            var integerPercent = (int) (buyableDamagePercent * 100);
            damagePercentText.text = $"+{integerPercent}%";
        }
        
        void IncreaseNextDamageAmplificationCost()
        {
            damageCost += 10;
            damageCostText.text = damageCost.ToString();
        }

        IncreaseNextDamageAmplification();
        IncreaseNextDamageAmplificationCost();
    }
}
